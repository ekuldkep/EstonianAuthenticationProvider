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
        public MobileIdServiceConstants MobileIdServiceConstants;
        public DigiDocServiceHelper DigiDocService { get; set; }

        public MobileIdHelper(DigiDocServiceHelper digiDocService, MobileIdServiceConstants mobileIdServiceConstants)
        {
            DigiDocService = digiDocService;
            MobileIdServiceConstants = mobileIdServiceConstants;
        }
        public virtual MobileAuthResultDto MobileIdAuthenticate(string idCode, string phoneNr)
        {
            var digiDocService = DigiDocService.ReturnDigiDocService();
            var result = new MobileAuthResultDto();
            var response = digiDocService.MobileAuthenticateAsync(new MobileAuthenticateRequest(idCode,
                MobileIdServiceConstants.CountryCode,
                phoneNr,
                MobileIdServiceConstants.Language,
                MobileIdServiceConstants.ServiceName,
                MobileIdServiceConstants.MessageToDisplay,
                MobileIdServiceConstants.SpChallange,
                MobileIdServiceConstants.MessagingMode,
                MobileIdServiceConstants.AsyncConfiguration,
                MobileIdServiceConstants.ReturnCertData,
                MobileIdServiceConstants.ReturnRevocationData
                 ));

            if (response.Result.Status == CertificateStatusConstants.MobileIdStatusOk)
            {
                result.SkMobileIdAuthenticateStatus = response.Result.Status;
                result.ChallengeID = response.Result.ChallengeID;
                result.IdCode = response.Result.UserIDCode;
                result.SessionCode = response.Result.Sesscode;
                result.FirstName = response.Result.UserGivenname;
                result.LastName = response.Result.UserSurname;
                result.UserCountry = response.Result.UserCountry;
                result.CertificateData = response.Result.CertificateData;
                result.Challenge = response.Result.Challenge;
                result.RevocationData = response.Result.RevocationData;
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
