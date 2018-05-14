using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using DigiDocService;
using TestAuthPackage.Constants;

namespace TestAuthPackage.Helpers
{
    public class DigiDocServiceHelper
    {
        public DigiDocServiceVariables DigiDocServiceVariables;

        public DigiDocServiceHelper(DigiDocServiceVariables digiDocServiceVariables)
        {
            DigiDocServiceVariables = digiDocServiceVariables;
        }

        public virtual DigiDocServicePortTypeClient ReturnDigiDocService()
        {
            // https://stackoverflow.com/a/3704312
            var binding = new BasicHttpBinding();
            binding.Name = DigiDocServiceVariables.ServiceName;
            binding.Security.Mode = DigiDocServiceVariables.SecurityMode;
            var endpoint = new EndpointAddress(DigiDocServiceVariables.EndpointUrl);
            return new DigiDocServicePortTypeClient(binding, endpoint);
        }
    }
}
