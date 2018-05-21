using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using DigiDocService;
using EstonianAuthenticationProvider.Constants;

namespace EstonianAuthenticationProvider.Helpers
{
    public class DigiDocServiceHelper
    {
        public DigiDocServiceConfig DigiDocServiceConfig;

        public DigiDocServiceHelper(DigiDocServiceConfig digiDocServiceConfig)
        {
            DigiDocServiceConfig = digiDocServiceConfig;
        }

        public virtual DigiDocServicePortTypeClient ReturnDigiDocService()
        {
            // https://stackoverflow.com/a/3704312
            var binding = new BasicHttpBinding();
            binding.Name = DigiDocServiceConfig.ServiceName;
            binding.Security.Mode = DigiDocServiceConfig.SecurityMode;
            var endpoint = new EndpointAddress(DigiDocServiceConfig.EndpointUrl);
            return new DigiDocServicePortTypeClient(binding, endpoint);
        }
    }
}
