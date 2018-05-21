using System;
using System.Collections.Generic;
using System.Text;
using EstonianAuthenticationProvider.Constants;

namespace EstonianAuthenticationProvider.Dtos
{
    public class AuthenticationDto
    {
        public string IdCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserCountry { get; set; }
        public string UserOrganisation { get; set; }
        public string UserCN { get; set; }
        public string IssuerCN { get; set; }
        public string KeyUsage { get; set; }
        public string Status { get; set; }
        public string RevocationData { get; set; }
        public string EnhancedKeyUsage { get; set; }
        public string Secret { get; set; }
        public string SkMobileIdAuthenticateStatus { get; set; }
        public string ChallengeID { get; set; }
        public int SessionCode { get; set; }
        public string CertificateData { get; set; }
        public string Challenge { get; set; }
        public MobileIdAuthResultType MobileIdAuthResultType { get; set; }
        public string Message { get; set; }
        public bool IsMobileIdValid { get; set; }
        public string ErrorCode { get; set; }

        public string Serialize()
        {
            return $"{IdCode},{FirstName},{LastName}," +
                   $"{UserCountry},{UserOrganisation}," +
                   $"{Secret}";
        }

        public static AuthenticationDto DeSerialize(string serializedDto)
        {
            var dto = new AuthenticationDto();
            var variables = serializedDto.Split(',');
            dto.IdCode = variables[0];
            dto.FirstName = variables[1];
            dto.LastName = variables[2];

            dto.UserCountry = variables[3];
            dto.UserOrganisation = variables[4];
            dto.Secret = variables[5];
            return dto;
        }
    }
}
