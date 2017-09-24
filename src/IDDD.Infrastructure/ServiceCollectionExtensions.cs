﻿using IDDD.Infrastructure.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IDDD.Infrastructure
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddMongoDBContext(this IServiceCollection services, IConfiguration config)
        {

            services.AddTransient<IMongoConnection>(_ =>
            {
                return new MongoConnection(config);
            });

            services.AddScoped<IMongoContext, MongoContext<IMongoConnection>>();
            return services;
        }
        
    }
}
