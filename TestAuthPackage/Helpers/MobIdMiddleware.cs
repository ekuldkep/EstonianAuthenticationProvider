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
    // TODO: should not named middleware, controller class should not have all the nice functions, separate them into a separate class
    public class MobIdMiddleware : Controller
    {
        public static string MobileIdAuthStatusKey = "MobileIdAuthStatus";
        public static string BaseUri = "";
        public MobileIdConstants MobileIdConstants;

        public MobIdMiddleware(IOptions<MobileIdConstants> mobileIdConstants)
        {
            MobileIdConstants = mobileIdConstants.Value;
        }
        public virtual MobileIdHelper GetClient()
        {
            return new MobileIdHelper(GetDigiDocService(), MobileIdConstants);
        }

        public virtual DigiDocService GetDigiDocService()
        {
            return new DigiDocService();
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

        // TODO: change name, It does not login, it starts the flow
        // The untouched information is stored in session to prevent malicious attacks
        public MobileAuthResultDto MobileIdLogin(string idCode, string phoneNr)
        {
            phoneNr = TrimPhoneNr(phoneNr);
            var mobileIdResult = MobileIdLoginClient(idCode, phoneNr);

            var serialisedResult = JsonConvert.SerializeObject(mobileIdResult);
            HttpContext.Session.SetString(MobileIdAuthStatusKey, serialisedResult);

            return mobileIdResult;
        }

        // TODO: magic strings to config class enum or something
        // TODO: is not a client, even though it uses a client, lets rename it
        private MobileAuthResultDto MobileIdLoginClient(string idCode, string phoneNr)
        {
            var mobileIdResult = GetClient().MobileIdAuthenticate(idCode, phoneNr);

            var isOk = mobileIdResult.SkMobileIdAuthenticateStatus == "OK";

            mobileIdResult.AuthenticationResultType =
                isOk ? AuthenticationResultType.Pending : AuthenticationResultType.Failed;
            mobileIdResult.IsMobileIdValid = isOk;
            return mobileIdResult;
        }

        // Doesn't need any arguments as the session holds the IdAuth session also
        // without a session null is returned
        public MobileAuthResultDto PollMobileIdLogInStatus()
        {
            var sessionValueString = HttpContext.Session.GetString(MobileIdAuthStatusKey);

            var mobileAuthTrustedInitialData = JsonConvert.DeserializeObject<MobileAuthResultDto>(sessionValueString);

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
                HttpContext.Session.Remove(MobileIdAuthStatusKey);
            }

            return mobileAuthCurrentStatus;
        }

        public string GenerateAuthUrl(MobileAuthResultDto authResult)
        {
            string token = "this will be crypted and contains all the necessray data like in IdCard auth";
            return $"{BaseUri}{token}";
        }

        // TODO: magic strings
        private MobileAuthResultDto GetMobileIdAuthenticateStatus(MobileAuthResultDto mobileAuthTrustedInitialData)
        {
            var result = GetClient().GetMobileIdAuthenticateStatus(mobileAuthTrustedInitialData.SessionCode);

            if (result == "USER_AUTHENTICATED")
            {
                mobileAuthTrustedInitialData.AuthenticationResultType = AuthenticationResultType.Succeeded;
            }
            else if (result == "OUTSTANDING_TRANSACTION")
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

        public JsonResult InitilizeMobileAuthJson(string idCode, string phoneNr)
        {
            var mobileAuthResult = MobileIdLogin(idCode, phoneNr);
            return Json(new
            {
                challenge=mobileAuthResult.Challenge,
                status=mobileAuthResult.AuthenticationResultType,
            });
        }

        public JsonResult PollMobileAuthStatusJson()
        {
            var mobileAuthResult = PollMobileIdLogInStatus();
            if (mobileAuthResult == null)
            {
                return Json(new
                {
                    status=AuthenticationResultType.Failed,
                });
            }

            string url = GenerateAuthUrl(mobileAuthResult);
            return Json(new
            {
                url=url,
            });
        }
    }
}
