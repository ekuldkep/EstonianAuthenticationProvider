﻿using System;
using System.Collections.Generic;
using System.Text;
using BLL.Helpers;
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

        public MobileIdControllerHelper(IOptions<MobileIdServiceConstants> mobileIdServiceConstants, IOptions<MobileIdConfiguration> mobileIdConfiguration)
        {
            MobileIdServiceConstants = mobileIdServiceConstants.Value;
            MobileIdConfiguration = mobileIdConfiguration.Value;
        }
        public virtual MobileIdHelper GetClient()
        {
            return new MobileIdHelper(GetDigiDocService(), MobileIdServiceConstants);
        }

        public virtual DigiDocServiceHelper GetDigiDocService()
        {
            return new DigiDocServiceHelper();
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
        public virtual MobileAuthResultDto MobileIdLoginStart(string idCode, string phoneNr)
        {
            phoneNr = TrimPhoneNr(phoneNr);
            var mobileIdResult = MobileIdLoginRequest(idCode, phoneNr);

            var serialisedResult = JsonConvert.SerializeObject(mobileIdResult);
            HttpContext.Session.SetString(MobileIdConfiguration.MobileIdAuthStatusKey, serialisedResult);

            return mobileIdResult;
        }

        public virtual MobileAuthResultDto MobileIdLoginRequest(string idCode, string phoneNr)
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
        public virtual MobileAuthResultDto PollMobileIdLogInStatus()
        {
            var sessionValueString = HttpContext.Session.GetString(MobileIdConfiguration.MobileIdAuthStatusKey);

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
                HttpContext.Session.Remove(MobileIdConfiguration.MobileIdAuthStatusKey);
            }

            return mobileAuthCurrentStatus;
        }

        public virtual string GenerateAuthUrl(MobileAuthResultDto authResult)
        {
            string token = SecurityHelper.EncryptString(authResult.PropertyValuesCommaSeparated(), EncryptionKey.Key());
            return $"{MobileIdConfiguration.RedirectUrl}{token}";
        }

        public virtual MobileAuthResultDto GetMobileIdAuthenticateStatus(MobileAuthResultDto mobileAuthTrustedInitialData)
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
                challenge=mobileAuthResult.Challenge,
                status=mobileAuthResult.AuthenticationResultType,
            });
        }

        public virtual JsonResult PollMobileAuthStatusJson()
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