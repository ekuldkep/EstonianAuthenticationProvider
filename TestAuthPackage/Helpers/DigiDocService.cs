using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using DigiDocService;

namespace TestAuthPackage.Helpers
{
    public class DigiDocService
    {
        private readonly string ServiceName = "DigiDocService";
        private readonly string EndpointUrl = "https://tsp.demo.sk.ee/dds.wsdl";
        private readonly BasicHttpSecurityMode SecurityMode = BasicHttpSecurityMode.Transport;
        private readonly TimeSpan CloseTimeout = TimeSpan.FromMinutes(1);
        private readonly TimeSpan OpenTimeout = TimeSpan.FromMinutes(1);
        private readonly TimeSpan SendTimeout = TimeSpan.FromMinutes(1);
        private readonly TimeSpan RecieveTimeout = TimeSpan.FromMinutes(10);
        private readonly bool AllowCookies = false;
        private readonly bool BypassProxyOnLocal = false;
        private readonly int MaxBufferSize = 65536;
        private readonly int MaxBufferPoolSize = 524288;
        private readonly TransferMode TransferMode = TransferMode.Buffered;
        private readonly Encoding TextEncoding = System.Text.Encoding.UTF8;
        private readonly bool UseDefaultWebProxy = true;
        private readonly int MaxDepth = 32;
        private readonly int MaxStringContentLength = 8192;
        private readonly int MaxArrayLength = 16384;
        private readonly int MaxBytesPerRead = 4096;
        private readonly int MaxNameTableCharCount = 8192;
        private readonly HttpClientCredentialType ClientCredentialType = HttpClientCredentialType.None;
        private readonly HttpProxyCredentialType ProxyCredentialType = HttpProxyCredentialType.None;
        private readonly string Realm = "";
        private readonly BasicHttpMessageCredentialType BasicClientCredentialType = BasicHttpMessageCredentialType.UserName;

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
