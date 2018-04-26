using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using DigiDocService;

namespace TestAuthPackage.Helpers
{
    public class DigiDocService
    {
        public string ServiceName = "DigiDocService";
        public string EndpointUrl = "https://tsp.demo.sk.ee/dds.wsdl";
        public BasicHttpSecurityMode SecurityMode = BasicHttpSecurityMode.Transport;

        public DigiDocService(string serviceName, string endpointUrl, BasicHttpSecurityMode securityMode)
        {
            ServiceName = serviceName;
            EndpointUrl = endpointUrl;
            SecurityMode = securityMode;
        }

        public DigiDocService()
        {

        }
        public virtual DigiDocServicePortTypeClient ReturnDigiDocService()
        {
            // https://stackoverflow.com/a/3704312
            var binding = new BasicHttpBinding();
            binding.Name = ServiceName;
            binding.Security.Mode = SecurityMode;
            var endpoint = new EndpointAddress(EndpointUrl);
            return new DigiDocServicePortTypeClient(binding, endpoint);
        }
    }
}
