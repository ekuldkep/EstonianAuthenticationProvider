using System;
using System.Collections.Generic;
using System.Text;
using EstonianAuthenticationProvider.Constants;
using EstonianAuthenticationProvider.Dtos;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EstonianAuthenticationProvider.Helpers
{
    public static class UnpackHelper
    {
        // decrypt and blacklist check for token
        public static AuthenticationDto UnpackDto(String key, HttpContext httpContext)
        {
            var secretLocal = httpContext.Session.GetString(SecurityConstants.ClientSecret);

            var token = httpContext.Request.Query["token"].ToString();

            var decryptedToken = SecurityHelper.DecryptString(token, key);

            var authenticationDto =  AuthenticationDto.DeSerialize(decryptedToken);

            if (authenticationDto.Secret == secretLocal)
            {
                return null;
            }
            // TODO: DELETE ClientSecret from Session
            return authenticationDto;
        }
    }
}
