# About
Download EstonianAuthenticationProvider from nuget packet manager https://www.nuget.org/packages/EstonianAuthenticationProvider/.

Solution works on ASP.NET Core web applications with implementation of .Net Core starting with version 2.0 or with implementation of .NET Framework starting with version 4.6.1 with SDK 2.x. To use full solution it is recommended to use MCV pattern but many methods can be used without.

# To use ID-card functionality

1. Configure your webserver so that the server would ask for certificate. Solution works with IIS, Apache and Nginx. Some examples how to configure web server
    -  https://www.id.ee/public/ID_kaardi_toe_seadistamine_IIS_veebiserverile_EST.pdf
    -  https://www.id.ee/public/Configuring_Apache_web_server_to_support_ID.pdf;
2. Make new project (can be inside the same solution) so that it can be configured to ask for client certificate;
3. Make new Middleware class in that project and let it inherit the package CertificateAuthenticationHelper class;
4. Add your Middleware class into the pipleline by registering it in the Startup class, Configure method;
5. Also register classes CertificateValidationConfig and DigiDocServiceConfig in the Startup class, ConfigureService method. For example if you want to use Options patter JSON option it should look something like this:

    ```csharp
    services.Configure<CertificateValidalidationConfig>(Configuration.GetSection("CertificateValidation"));
    services.Configure<DigiDocServiceConfig>(Configuration.GetSection("DigiDocServiceVariables"));
    ```
    
     And then add those sections to the appsettings.json class.  
    CertificateValidation section has mandatory value - RedirectUrl.  
    Other values are already set unless you want to override those.  
    In DigiDocServiceVariables are no mandatory values.                                

6. In your middleware class constructor add values to the CertificateAuthenticationHelper class variables like this:
    ```csharp
    public IdAuthMiddleware(
        RequestDelegate next,
        IOptions<CertificateValidalidationConfig> certificateValidalidationConfig,
        IOptions<DigiDocServiceConfig> digiDocServiceVariables
        )
    {
        _next = next;
        CertificateValidalidationConfig = certificateValidalidationConfig.Value;
        DigiDocServiceConfig = digiDocServiceVariables.Value;
    }
    ```
7. In your middleware class in method Invoke, call out for the CertificateAuthenticationHelper class method InvokeCert. Like this: 
    public async Task Invoke(HttpContext httpContext)
     ```csharp
    {
        await InvokeCert(httpContext);
        await _next(httpContext);
    }
    ```
    

# To use Mobile-ID functionality
1. Make new Controller class
2. Let it inherit the MobileIdControllerHelper class
3. Register classes MobileIdConfig, MobileIdHelperConfig, and DigiDocServiceConfig in the Startup class, ConfigureService method. For example if you want to use Options patter JSON option it should look something like this:
     ```csharp
        services.Configure<MobileIdConfig>(Configuration.GetSection("MobileIdConfiguration"));
        services.Configure<MobileIdHelperConfig>(Configuration.GetSection("MobileIdServiceConstants"));
        services.Configure<DigiDocServiceConfig>(Configuration.GetSection("DigiDocServiceVariables"));
    ```
4. And then add those sections to the appsettings.json class.
5. In your Controller class constructor add values to the MobileIdControllerHelper class variables like this:
    ```csharp
    public HomeController(IOptions<MobileIdConfig> mobileIdConfig, 
        IOptions<MobileIdHelperConfig> mobileIdHelperConfig, 
        IOptions<DigiDocServiceConfig> digiDocServiceConfig)
    {
        MobileIdConfiguration = mobileIdConfig.Value;
        MobileIdServiceConstants = mobileIdHelperConfig.Value;
        DigiDocServiceConfig = digiDocServiceConfig.Value;
    }
    ```
6. Call out method InitializeMobileAuthJson to start Mobile-ID Authentication:
    ```csharp
    public IActionResult Index(string idCode, string phoneNr){
        InitializeMobileAuthJson(idCode, phoneNr)
        return View();
    }
    ```
7. Optinally, write Javascript code to poll method PollMobileAuthStatusJason
