using System;
using System.Collections.Generic;
using System.Text;
using TestAuthPackage.Constants;

namespace TestAuthPackage.Dtos
{
    public class CertificationInfoDto
    {
        public bool CanRedirect { get; set; }
        public bool IsCertificateValid { get; set; }
        public AuthenticationDto AuthenticationDto { get; set; }
        public CertificateChainStatusEnum CertifikateChainStatus { get; set; }
        public string CertificateStatus { get; set; }
    }
}
