using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace EstonianAuthenticationProvider.Constants
{
    public class DigiDocServiceConfig
    {
        public string ServiceName { get; set; } = "DigiDocService";
        public string EndpointUrl { get; set; } = "https://tsp.demo.sk.ee/dds.wsdl";
        public BasicHttpSecurityMode SecurityMode { get; set; } = BasicHttpSecurityMode.Transport;
        public TimeSpan CloseTimeout { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan OpenTimeout { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan SendTimeout { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan RecieveTimeout { get; set; } = TimeSpan.FromMinutes(10);
        public bool AllowCookies { get; set; } = false;
        public bool BypassProxyOnLocal { get; set; } = false;
        public int MaxBufferSize { get; set; } = 65536;
        public int MaxBufferPoolSize { get; set; } = 524288;
        public TransferMode TransferMode { get; set; } = TransferMode.Buffered;
        public Encoding TextEncoding { get; set; } = System.Text.Encoding.UTF8;
        public bool UseDefaultWebProxy { get; set; } = true;
        public int MaxDepth { get; set; } = 32;
        public int MaxStringContentLength { get; set; } = 8192;
        public int MaxArrayLength { get; set; } = 16384;
        public int MaxBytesPerRead { get; set; } = 4096;
        public int MaxNameTableCharCount { get; set; } = 8192;
        public HttpClientCredentialType ClientCredentialType { get; set; } = HttpClientCredentialType.None;
        public HttpProxyCredentialType ProxyCredentialType { get; set; } = HttpProxyCredentialType.None;
        public string Realm { get; set; } = "";
        public BasicHttpMessageCredentialType BasicClientCredentialType { get; set; } = BasicHttpMessageCredentialType.UserName;
    }
}
