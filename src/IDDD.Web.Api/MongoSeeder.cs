using IDDD.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Core;
using OpenIddict.Models;
using OpenIddict.Mongo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mvc.Server
{
    public static class MongoSeeder
    {
        public static async Task InitializeMongoClientsAsync(IServiceProvider services, CancellationToken cancellationToken)
        {
            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IMongoOpenIddictContext>();

                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication>>();

                if (await manager.FindByClientIdAsync("idddWeb", cancellationToken) == null)
                {
                    var application = new OpenIddictApplication
                    {
                        ClientId = "idddWeb",
                        DisplayName = "IDDD Web client application",
                        LogoutRedirectUri = "http://localhost:5000/",
                        RedirectUri = "http://localhost:5000/",
                        AllowedOrigin = "http://localhost:5000/",
                        Active = true,
                        Type = "Public"
                    };

                    await manager.CreateAsync(application, string.Empty, cancellationToken);
                }
            }
        }
    }
}
