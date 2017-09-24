﻿using IDDD.Core.Cqs.Command;
using IDDD.Core.Cqs.Query;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace IDDD.App.Cqs
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
        {

            services.Scan(scan => scan
            .FromAssembliesOf(typeof(ServiceCollectionExtensions))
            .AddClasses(classes => classes.AssignableToAny(typeof(IHandleQueryAsync<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());


            return services;
        }

        public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(ServiceCollectionExtensions))
                .AddClasses(classes => classes.AssignableToAny(typeof(IAsyncCommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
                );


            return services;
        }
    }
}
