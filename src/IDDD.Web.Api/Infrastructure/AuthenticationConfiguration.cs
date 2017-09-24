using System;
using System.Net.Http;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using IDDD.App;
using AspNet.Security.OAuth.Validation;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using AspNet.Security.OpenIdConnect.Primitives;

namespace IDDD.Web.Api.Infrastructure
{
    public static class AuthenticationConfiguration
    {
        public static IApplicationBuilder ConfigureAuthentication(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseWhen(IsApi, ApiAuthentication(configuration));
            app.UseWhen(IsNonApi, NonApiAuthentication(configuration));
            app.UseOpenIddict();
            return app;
        }

        private static bool IsApi(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments(new PathString("/api"));
        }

        private static bool IsNonApi(HttpContext context)
        {
            return !context.Request.Path.StartsWithSegments(new PathString("/api"));
        }

        private static Action<IApplicationBuilder> ApiAuthentication(IConfiguration configuration)
        {
            // If you prefer using JWT, don't forget to disable the automatic
            // JWT -> WS-Federation claims mapping used by the JWT middleware:

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            return branch => branch.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                Authority = configuration.AuthorityHostName(),
                Audience = configuration.ApiHostName(),
                RequireHttpsMetadata = false,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = OpenIdConnectConstants.Claims.Subject,
                    RoleClaimType = OpenIdConnectConstants.Claims.Role
                }
            });

            // If you prefer OAuthValidation
            //return branch => branch.UseOAuthValidation();

        }

        private static Action<IApplicationBuilder> NonApiAuthentication(IConfiguration configuration)
        {
            return branch => branch.UseIdentity();
        }
    }
}
