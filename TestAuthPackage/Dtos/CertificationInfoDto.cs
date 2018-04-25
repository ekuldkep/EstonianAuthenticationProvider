using System;
using System.Collections.Generic;
using System.Text;

namespace TestAuthPackage.Dtos
{
    public class CertificationInfoDto
    {
        public bool IsCertificateValid { get; set; }
        public AuthenticationDto AuthenticationDto { get; set; }
        public bool IsCertificationFromValidRoot { get; set; }
    }
}
