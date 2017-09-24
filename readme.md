# Token-authentication/authorization in .Net Core with OpenIddict and MongoDB #

### Summary ###
Sample shows how you can use [OpenIddict](https://github.com/openiddict/openiddict-core) framework developed by @Kevin Chalet for integrating token-based authentication/authorization in AspNet Core.   

Built-in implementation developed by @Kevin Chalet uses EntityFramework Core extensions with the out-of-the-box .Net Core Identity EF stores.

This sample include two adapters for the purpose to use MongoDB as persistance layer for both .Net Core Identity and OpenIddict tokens store.

For the .Net Core Identity, I've implemented base internal interfaces - UserStore`<`IdentityUser`>` and RoleStore`<`IdentityRole`>`. Both are resposible for managing auth functionalities like registrations,two-factor auth, password recovery and verifications (email, token...).

For the OpenIddict extensions, I've plugged in persistance adapter for the core interfaces - OpenIddictApplicationStore, OpenIddictAuthorizationStore, OpenIddictScopeStore, OpenIddictTokenStore. OpenIddict.Models are also extended with additional properties for functionalities like client deactivation and client origin validation.

Project is developed as part of custom authorization functionality for securing web clients by incorporating OAuth password-credentials flow and some additional layering as base skeleton.

Who ever needs only ported implementation of the OpenIddict clients/token store should check this link:

https://github.com/ilkerkarimanov/openiddict-core/tree/mongo-1.0.1

### Prerequisites ###
 - In order to run this sample should install MongoDB instance and apply necessary changes in appsettings.json
 - Testing the endpoints can be done with Postman - postman-readme.
 
### Solution ###
Solution | Author(s)
---------|----------
OpenIddict.Mongo | Ilker Karimanov

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | September 2017 | Initial release

### Future improvements

*PS: Any suggestions will be greatly appreciated*

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Q/A #
**Q**: Why using OpenIddict? 

**A**: Personally, I use pure implementation of the OpenIdConnect.Server project (since beta-4...) by @Kevin Chalet for all my custom flows. Recently I've checked that he has done extension(OpenIddict) of it, which can be easily plugged into out-of-the-box Identity stack - so why not to do it for MongoDB too.

**Q**: What is the purpose of that multi-layer/project separation?

**A**: Better control of what is part of your solution interfaces(In) and what is supporting your core implementation(Out).

**Q**: What is the purpose of OriginAllowed field in OpenIddict.Models.OpenIddictApplication?

**A**: It is expected if user as successfully authenticated by username/password at least to be associated with some security domain, something which identifies the client/origin. Client interface store information for the origin which is critical if we want to secure against unknow cross-origin requests.

# Code Samples #

## Identity Configuration ##
Code snippet:
```C#
...

            services.AddMongoDBContext(_configuration);

            services.AddMongoDomainPersistance(_configuration);

            services.AddIdentityWithMongoStores();

```
## OpenIddict Configuration ##     
Code snippet:
```C#
...
            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            services.AddMongoDBContext<MongoOpenIddictContext, IDDDOpenIddictConnection>();

            // Register the OpenIddict services.
            services.AddOpenIddict(options =>
            {
                // Register the Entity Framework stores.
                //options.AddEntityFrameworkCoreStores<ApplicationDbContext>();

                options.AddMongoCoreStores();

                // Register the ASP.NET Core MVC binder used by OpenIddict.
                // Note: if you don't call this method, you won't be able to
                // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                options.AddMvcBinders();

                // Enable the authorization, logout, token and userinfo endpoints.
                options.EnableLogoutEndpoint("/connect/logout")
                       .EnableTokenEndpoint("/connect/token")
                       .EnableUserinfoEndpoint("/api/userinfo");

                // Note: the Mvc.Client sample only uses the code flow and the password flow, but you
                // can enable the other flows if you need to support implicit or client credentials.
                options.AllowPasswordFlow()
                       .AllowRefreshTokenFlow();

                // Make the "client_id" parameter mandatory when sending a token request.
                options.RequireClientIdentification();

                // When request caching is enabled, authorization and logout requests
                // are stored in the distributed cache by OpenIddict and the user agent
                // is redirected to the same page with a single parameter (request_id).
                // This allows flowing large OpenID Connect requests even when using
                // an external authentication provider like Google, Facebook or Twitter.
                options.EnableRequestCaching();

                // During development, you can disable the HTTPS requirement.
                options.DisableHttpsRequirement();

                // Note: to use JWT access tokens instead of the default
                // encrypted format, the following lines are required:
                //
                 options.UseJsonWebTokens();
                 options.AddEphemeralSigningKey();
            });
```

## Authentication Configuration ##

Code snippet:
```C#
...
            app.UseWhen(IsApi, ApiAuthentication(configuration));
            app.UseWhen(IsNonApi, NonApiAuthentication(configuration));
            app.UseOpenIddict();
```

## Api Authentication ##

Code snippet:
```C#
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
```

## CORS ##
```C#
...
			//intercept connection's client origin
            Request.HttpContext.Items.Add("as:clientAllowedOrigin", result.AllowedOrigin);
...
			//set client origin into the response access control
            var allowedOrigin = Request.HttpContext.Items["as:clientAllowedOrigin"] as string;
            if (allowedOrigin != null)
            {
                if (Request.HttpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                {
                    Request.HttpContext.Response.Headers["Access-Control-Allow-Origin"] = allowedOrigin;
                }
                else
                {
                    Request.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", new StringValues(allowedOrigin));
                }
            }
...

```

