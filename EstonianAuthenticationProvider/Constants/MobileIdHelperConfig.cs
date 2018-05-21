using System;
using System.Collections.Generic;
using System.Text;

namespace EstonianAuthenticationProvider.Constants
{
    public class MobileIdHelperConfig
    {
        public string CountryCode { get; set; } = "";
        public string Language { get; set; } = "EST";
        public string ServiceName { get; set; } = "Testimine";
        public string MessageToDisplay { get; set; } = "Test";
        public string SpChallange { get; set; } = null;
        public string MessagingMode { get; set; } = "asynchClientServer";
        public int AsyncConfiguration { get; set; } = 0;
        public bool ReturnCertData { get; set; } = false;
        public bool ReturnRevocationData { get; set; } = false;
    }
}
