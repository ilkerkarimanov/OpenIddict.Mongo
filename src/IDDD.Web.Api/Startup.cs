using System;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IDDD.Core;
using IDDD.Infrastructure;
using IDDD.Infrastructure.Mongo;
using IDDD.Core.Cqs;
using IDDD.App;
using IDDD.App.Cqs;
using Microsoft.Extensions.Logging;
using System.Threading;
using OpenIddict.Mongo.Core;
using Expandable.Infrastructure.Membership.OpenIddict;
using IDDD.Web.Api.Infrastructure;

namespace IDDD.Web.Api
{
    public class Startup
    {

        private const string defaultName = "default";

        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        private readonly IConfigurationRoot _configuration;
        private IServiceProvider _serviceProvider { get; set; }
        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            builder.AddEnvironmentVariables();

            _configuration = builder.Build();
            _env = env;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IConfiguration>(_ =>
            {
                return _configuration;
            });

            
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            services.AddMvc();

            services.AddCors(ConfigureCors);

            services.AddSharedKernel();

            services.AddMongoDBContext(_configuration);

            services.AddMongoDomainPersistance(_configuration);

            services.AddIdentityWithMongoStores();

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

            services.AddCqs();

            services.AddMailing(_configuration);

            services.AddCommandHandlers();

            services.AddQueryHandlers();

            //at last build service container
            _serviceProvider = services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole((text, logLevel) => logLevel >= LogLevel.Warning);

            app.ConfigureAuthentication(_configuration);

            app.UseCors(defaultName);
            
            app.UseMvc();

            Mvc.Server.MongoSeeder.InitializeMongoClientsAsync(app.ApplicationServices, CancellationToken.None).GetAwaiter().GetResult();

        }

        private static void ConfigureCors(Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions options)
        {
            options.AddPolicy(defaultName, policy =>
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials());
        }
    }
}
