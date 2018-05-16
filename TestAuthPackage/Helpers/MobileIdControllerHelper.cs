using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TestAuthPackage.Constants;
using TestAuthPackage.Dtos;

namespace TestAuthPackage.Helpers
{
    public class MobileIdControllerHelper : Controller
    {
        public MobileIdConfiguration MobileIdConfiguration;
        public MobileIdServiceConstants MobileIdServiceConstants;
        public DigiDocServiceVariables DigiDocServiceVariables;

        public virtual MobileIdHelper GetClient()
        {
            return new MobileIdHelper(GetDigiDocService(), MobileIdServiceConstants);
        }

        public virtual DigiDocServiceHelper GetDigiDocService()
        {
            return new DigiDocServiceHelper(DigiDocServiceVariables);
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

            mobileIdResult.AuthenticationResultType =
                isOk ? AuthenticationResultType.Pending : AuthenticationResultType.Failed;
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

            // return the dto with the arg AuthenticationResultType set to AuthenticationResultType.Failed when failed otherwise when success AuthenticationResultType.Succeeded
            var mobileAuthCurrentStatus = GetMobileIdAuthenticateStatus(
                mobileAuthTrustedInitialData
            );

            var authStatus = mobileAuthTrustedInitialData.AuthenticationResultType;

            // polling has finished
            if (authStatus != AuthenticationResultType.Pending)
            {
                HttpContext.Session.Remove(MobileIdConfiguration.MobileIdAuthStatusKey);
            }

            return mobileAuthCurrentStatus;
        }

        public virtual string GenerateAuthUrl(AuthenticationDto authResultDto)
        {
            var secret = HttpContext.Session.GetString(SecurityConstants.ClientSecret);

            authResultDto.Secret = secret;

            var serialisedResult = JsonConvert.SerializeObject(authResultDto);

            string token = SecurityHelper.EncryptString(serialisedResult, EncryptionKey.Key());
            return $"{MobileIdConfiguration.RedirectUrl}{token}";
        }

        public virtual AuthenticationDto GetMobileIdAuthenticateStatus(AuthenticationDto mobileAuthTrustedInitialData)
        {
            var result = GetClient().GetMobileIdAuthenticateStatus(mobileAuthTrustedInitialData.SessionCode);

            if (result == MobileIdResultConstants.UserAuthenticated)
            {
                mobileAuthTrustedInitialData.AuthenticationResultType = AuthenticationResultType.Succeeded;
            }
            else if (result == MobileIdResultConstants.OutstandingTransaction)
            {
                mobileAuthTrustedInitialData.AuthenticationResultType = AuthenticationResultType.Pending;
            }
            else
            {
                mobileAuthTrustedInitialData.AuthenticationResultType = AuthenticationResultType.Failed;
            }

            mobileAuthTrustedInitialData.Message = result;
            return mobileAuthTrustedInitialData;
        }

        public virtual JsonResult InitilizeMobileAuthJson(string idCode, string phoneNr)
        {
            var mobileAuthResult = MobileIdLoginStart(idCode, phoneNr);
            return Json(new
            {
                challenge=mobileAuthResult.ChallengeID,
                status=mobileAuthResult.AuthenticationResultType,
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
                    status=AuthenticationResultType.Failed,
                });
            }

            if (mobileAuthResult.AuthenticationResultType == AuthenticationResultType.Succeeded)
            {
                string url = GenerateAuthUrl(mobileAuthResult);
                return Json(new
                {
                    url = url,
                    status = mobileAuthResult.AuthenticationResultType,
                });
            }

            return Json(new
            {
                status = mobileAuthResult.AuthenticationResultType,
            });
        }
    }
}
