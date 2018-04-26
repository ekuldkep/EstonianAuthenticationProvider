using System;
using System.Collections.Generic;
using System.Text;

namespace TestAuthPackage.Constants
{
    public enum AuthenticationResultType
    {
        Succeeded = 1,
        InvalidCertificate = 2,
        Pending = 3,
        Failed = 4
    }
}
