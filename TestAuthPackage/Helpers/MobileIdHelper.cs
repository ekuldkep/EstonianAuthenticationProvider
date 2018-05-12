using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using DigiDocService;
using Microsoft.Extensions.Options;
using TestAuthPackage.Constants;
using TestAuthPackage.Dtos;

namespace TestAuthPackage.Helpers
{
    public class MobileIdHelper
    {
        public MobileIdConstants MobileIdConstants;
        public DigiDocService DigiDocService { get; set; }

        public MobileIdHelper(DigiDocService digiDocService, MobileIdConstants mobileIdConstants)
        {
            DigiDocService = digiDocService;
            MobileIdConstants = mobileIdConstants;
        }
        public virtual MobileAuthResultDto MobileIdAuthenticate(string idCode, string phoneNr)
        {
            var digiDocService = DigiDocService.ReturnDigiDocService();
            var result = new MobileAuthResultDto();
            var response = digiDocService.MobileAuthenticateAsync(new MobileAuthenticateRequest(idCode,
                MobileIdConstants.CountryCode,
                phoneNr,
                MobileIdConstants.Language,
                MobileIdConstants.ServiceName,
                MobileIdConstants.MessageToDisplay,
                MobileIdConstants.SpChallange,
                MobileIdConstants.MessagingMode,
                MobileIdConstants.AsyncConfiguration,
                MobileIdConstants.ReturnCertData,
                MobileIdConstants.ReturnRevocationData
                 ));

            if (response.Result.Status == CertificateStatusConstants.MobileIdStatusOk)
            {
                result.SkMobileIdAuthenticateStatus = response.Result.Status;
                result.ChallengeID = response.Result.ChallengeID;
                result.IdentificationCode = response.Result.UserIDCode;
                result.SessionCode = response.Result.Sesscode;
                result.GivenName = response.Result.UserGivenname;
                result.SurName = response.Result.UserSurname;
                result.UserCountry = response.Result.UserCountry;
                result.CertificateData = response.Result.CertificateData;
                result.Challenge = response.Result.Challenge;
                result.RevokationData = response.Result.RevocationData;
                result.UserCN = response.Result.UserCN;
                return result;
            }

            result.SkMobileIdAuthenticateStatus = response.Result.Status;
            return result;
        }

        public virtual string GetMobileIdAuthenticateStatus(int sessionCode)
        {
            var digiDocService = DigiDocService.ReturnDigiDocService();
            var response = digiDocService.GetMobileAuthenticateStatusAsync(new GetMobileAuthenticateStatusRequest(sessionCode, false));
            return response.Result.Status;
        }
        
    }
}
