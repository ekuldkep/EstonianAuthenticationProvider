using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using DigiDocService;
using EstonianAuthenticationProvider.Constants;
using EstonianAuthenticationProvider.Dtos;
using Microsoft.Extensions.Options;

namespace EstonianAuthenticationProvider.Helpers
{
    public class MobileIdHelper
    {
        public MobileIdHelperConfig MobileIdServiceConstants;
        public DigiDocServiceHelper DigiDocService { get; set; }

        public MobileIdHelper(DigiDocServiceHelper digiDocServiceHelper, MobileIdHelperConfig mobileIdServiceConstants)
        {
            DigiDocService = digiDocServiceHelper;
            MobileIdServiceConstants = mobileIdServiceConstants;
        }
        public virtual AuthenticationDto MobileIdAuthenticate(string idCode, string phoneNr)
        {
            var digiDocService = DigiDocService.ReturnDigiDocService();
            var result = new AuthenticationDto();
            try
            {
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
            }
            catch (AggregateException e)
            {
                result.ErrorCode = e.InnerExceptions.First().Message;
            }

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
