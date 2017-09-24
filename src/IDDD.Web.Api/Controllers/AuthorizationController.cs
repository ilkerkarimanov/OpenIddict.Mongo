using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using IDDD.App;
using IDDD.App.Cqs.Queries.Clients;
using IDDD.Core.Cqs.Command;
using IDDD.Core.Cqs.Query;
using IDDD.Domain.Membership;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using OpenIddict.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.Web.Api.Controllers
{
    public class AuthorizationController: BaseController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthorizationController(
            IQueryProcessor queryProcessor,
            ICommandDispatcher commandDispatcher,
            IOptions<IdentityOptions> identityOptions,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)

        {
            if (queryProcessor == null) throw new ArgumentNullException(nameof(queryProcessor));
            if (commandDispatcher == null) throw new ArgumentNullException(nameof(commandDispatcher));
            _queryProcessor = queryProcessor;
            _commandDispatcher = commandDispatcher;
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        #region Password, authorization code and refresh token flows
        // Note: to support non-interactive flows like password,
        // you must provide your own token endpoint action:

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange(OpenIdConnectRequest request)
        {
            Debug.Assert(request.IsTokenRequest(),
                "The OpenIddict binder for ASP.NET Core MVC is not registered. " +
                "Make sure services.AddOpenIddict().AddMvcBinders() is correctly called.");

            if (!request.IsRefreshTokenGrantType()
                && !request.IsPasswordGrantType())
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    ErrorDescription = "Only password and refresh token grant types " +
                                 "are accepted by this authorization server"
                });
            }

            var query = new ValidateClientQuery(request.ClientId, request.ClientSecret);
            var result = await _queryProcessor.ProcessAsync(query);

            if (!result.Succeeded)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidClient,
                    ErrorDescription = "Client not found in the database: ensure that your client_id is correct"
                });
            }

            Request.HttpContext.Items.Add("as:clientAllowedOrigin", result.AllowedOrigin);

            if (request.IsPasswordGrantType())
            {
                // Resolve ASP.NET Core Identity's user manager from the DI container.
                var user = await _userManager.FindByNameAsync(request.Username);

                if (user == null)
                {
                    return BadRequest(new OpenIdConnectResponse
                    {
                        Error = OpenIdConnectConstants.Errors.InvalidGrant,
                        ErrorDescription = "Invalid user details."
                    });
                }
            }

            if (request.IsPasswordGrantType() || request.IsRefreshTokenGrantType())
            {
                //Set CORS
                SetCorsHeader(request);

                // Retrieve the claims principal stored in the authorization code/refresh token.
                var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(
                    OpenIdConnectServerDefaults.AuthenticationScheme);
                // Resolve ASP.NET Core Identity's user manager from the DI container.
                var user = await _userManager.FindByNameAsync(request.Username);
                // Create a new authentication ticket, but reuse the properties stored in the
                // authorization code/refresh token, including the scopes originally granted.
                var ticket = await CreateTicketAsync(request, user, info.Properties);

                return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
            }

            return BadRequest(new OpenIdConnectResponse
            {
                Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported."
            });
        }
        #endregion


        /// <summary>
        /// Creates a valid authentication token used to create the access_token.
        /// </summary>
        private async Task<AuthenticationTicket> CreateTicketAsync(
                OpenIdConnectRequest request, IdentityUser user,
                AuthenticationProperties properties = null)
        {
            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(principal, properties,
                OpenIdConnectServerDefaults.AuthenticationScheme);

            if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
            {
                // Set the list of scopes granted to the client application.
                // Note: the offline_access scope must be granted
                // to allow OpenIddict to return a refresh token.
                ticket.SetScopes(new[]
                {
                    OpenIdConnectConstants.Scopes.OpenId,
                    OpenIdConnectConstants.Scopes.Email,
                    OpenIdConnectConstants.Scopes.Profile,
                    OpenIdConnectConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Roles
                }.Intersect(request.GetScopes()));
            }

            var configuration = Configuration();

            ticket.SetResources(configuration.ApiHostName());

            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            foreach (var claim in ticket.Principal.Claims)
            {
                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType)
                {
                    continue;
                }

                var destinations = new List<string>
                {
                    OpenIdConnectConstants.Destinations.AccessToken
                };

                // Only add the iterated claim to the id_token if the corresponding scope was granted to the client application.
                // The other claims will only be added to the access_token, which is encrypted when using the default format.
                if ((claim.Type == OpenIdConnectConstants.Claims.Name && ticket.HasScope(OpenIdConnectConstants.Scopes.Profile)) ||
                    (claim.Type == OpenIdConnectConstants.Claims.Email && ticket.HasScope(OpenIdConnectConstants.Scopes.Email)) ||
                    (claim.Type == OpenIdConnectConstants.Claims.Role && ticket.HasScope(OpenIddictConstants.Claims.Roles)))
                {
                    destinations.Add(OpenIdConnectConstants.Destinations.IdentityToken);
                }

                claim.SetDestinations(destinations);
            }

            return ticket;
        }

        /// <summary>
        /// Set cross-origin HTTP request (Cors) header to allow requests from a different domains. 
        /// This Cors value is specific to an Application and set by when validating the client application (ValidateClientAuthenticationp).
        /// </summary>
        private void SetCorsHeader(OpenIdConnectRequest context)
        {
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
        }

        public IConfiguration Configuration()
        {
            return Request.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
        }
    }
}
