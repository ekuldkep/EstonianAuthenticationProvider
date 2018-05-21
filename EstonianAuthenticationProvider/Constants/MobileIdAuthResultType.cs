using System;
using System.Collections.Generic;
using System.Text;

namespace EstonianAuthenticationProvider.Constants
{
    public enum MobileIdAuthResultType
    {
        Succeeded = 1,
        InvalidCertificate = 2,
        Pending = 3,
        Failed = 4
    }
}
