using System;
using System.Collections.Generic;
using System.Text;
using EstonianAuthenticationProvider.Constants;
using EstonianAuthenticationProvider.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EstonianAuthenticationProvider.Helpers
{
    public class MobileIdControllerHelper : Controller
    {
        public MobileIdConfig MobileIdConfiguration;
        public MobileIdHelperConfig MobileIdServiceConstants;
        public DigiDocServiceConfig DigiDocServiceConfig;

        public virtual MobileIdHelper GetClient()
        {
            return new MobileIdHelper(GetDigiDocService(), MobileIdServiceConstants);
        }

        public virtual DigiDocServiceHelper GetDigiDocService()
        {
            return new DigiDocServiceHelper(DigiDocServiceConfig);
        }

        public virtual string TrimPhoneNr(string phoneNumber)
        {
            var phoneNr = phoneNumber.Trim();

            if (phoneNr.StartsWith("372"))
                phoneNr = phoneNr.Insert(0, "+");

            if (!phoneNr.StartsWith("+372"))
                phoneNr = phoneNr.Insert(0, "+372");

            return phoneNr;
        }

        // The untouched information is stored in session to prevent malicious attacks
        public virtual AuthenticationDto MobileIdLoginStart(string idCode, string phoneNr)
        {
            phoneNr = TrimPhoneNr(phoneNr);
            var mobileIdResult = MobileIdLoginRequest(idCode, phoneNr);

            var serialisedResult = JsonConvert.SerializeObject(mobileIdResult);
            HttpContext.Session.SetString(MobileIdConfiguration.MobileIdAuthStatusKey, serialisedResult);
            return mobileIdResult;
        }

        public virtual AuthenticationDto MobileIdLoginRequest(string idCode, string phoneNr)
        {
            var mobileIdResult = GetClient().MobileIdAuthenticate(idCode, phoneNr);

            var isOk = mobileIdResult.SkMobileIdAuthenticateStatus == CertificateStatusConstants.MobileIdStatusOk;

            mobileIdResult.MobileIdAuthResultType =
                isOk ? MobileIdAuthResultType.Pending : MobileIdAuthResultType.Failed;
            mobileIdResult.IsMobileIdValid = isOk;
            return mobileIdResult;
        }

        // Doesn't need any arguments as the session holds the IdAuth session also
        // without a session null is returned
        public virtual AuthenticationDto PollMobileIdLogInStatus()
        {
            var sessionValueString = HttpContext.Session.GetString(MobileIdConfiguration.MobileIdAuthStatusKey);

            var mobileAuthTrustedInitialData = JsonConvert.DeserializeObject<AuthenticationDto>(sessionValueString);

            // we dont aknowledge polling without session
            if (mobileAuthTrustedInitialData == null)
            {
                return null;
            }

            // return the dto with the arg MobileIdAuthResultType set to MobileIdAuthResultType.Failed when failed otherwise when success MobileIdAuthResultType.Succeeded
            var mobileAuthCurrentStatus = GetMobileIdAuthenticateStatus(
                mobileAuthTrustedInitialData
            );

            var authStatus = mobileAuthTrustedInitialData.MobileIdAuthResultType;

            // polling has finished
            if (authStatus != MobileIdAuthResultType.Pending)
            {
                HttpContext.Session.Remove(MobileIdConfiguration.MobileIdAuthStatusKey);
            }

            return mobileAuthCurrentStatus;
        }

        public virtual string GenerateAuthUrl(AuthenticationDto authResultDto)
        {
            var secret = HttpContext.Session.GetString(SecurityConstants.ClientSecret);

            authResultDto.Secret = secret;

            var serialisedResult = authResultDto.Serialize();

            string token = SecurityHelper.EncryptString(serialisedResult, MobileIdConfiguration.Key);
            return $"{MobileIdConfiguration.RedirectUrl}{token}";
        }

        public virtual AuthenticationDto GetMobileIdAuthenticateStatus(AuthenticationDto mobileAuthTrustedInitialData)
        {
            var result = GetClient().GetMobileIdAuthenticateStatus(mobileAuthTrustedInitialData.SessionCode);

            if (result == MobileIdResultConstants.UserAuthenticated)
            {
                mobileAuthTrustedInitialData.MobileIdAuthResultType = MobileIdAuthResultType.Succeeded;
            }
            else if (result == MobileIdResultConstants.OutstandingTransaction)
            {
                mobileAuthTrustedInitialData.MobileIdAuthResultType = MobileIdAuthResultType.Pending;
            }
            else
            {
                mobileAuthTrustedInitialData.MobileIdAuthResultType = MobileIdAuthResultType.Failed;
            }

            mobileAuthTrustedInitialData.Message = result;
            return mobileAuthTrustedInitialData;
        }

        public virtual JsonResult InitilizeMobileAuthJson(string idCode, string phoneNr)
        {
            var mobileAuthResult = MobileIdLoginStart(idCode, phoneNr);
            return Json(new
            {
                challenge = mobileAuthResult.ChallengeID,
                status = mobileAuthResult.MobileIdAuthResultType,
                exceptionCode = mobileAuthResult.ErrorCode
            });
        }

        public virtual JsonResult PollMobileAuthStatusJson()
        {
            var mobileAuthResult = PollMobileIdLogInStatus();
            // no session found
            if (mobileAuthResult == null)
            {
                return Json(new
                {
                    status = MobileIdAuthResultType.Failed,
                });
            }

            if (mobileAuthResult.MobileIdAuthResultType == MobileIdAuthResultType.Succeeded)
            {
                string url = GenerateAuthUrl(mobileAuthResult);
                return Json(new
                {
                    url = url,
                    status = mobileAuthResult.MobileIdAuthResultType,
                });
            }

            return Json(new
            {
                status = mobileAuthResult.MobileIdAuthResultType,
            });
        }
    }
}
