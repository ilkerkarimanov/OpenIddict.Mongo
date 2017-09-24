using IDDD.App.Messaging;
using IDDD.Domain.Messaging;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IDDD.App
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMailing(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddTransient<IEmailSender, MessageSender>();

            return services;
        }

    }
}
