using IDDD.Common.Cqs.Command;
using IDDD.Common.Cqs.Query;
using Microsoft.Extensions.DependencyInjection;

namespace IDDD.Common.Cqs
{
    public static class ServiceCollectionExtensions
        {

            public static IServiceCollection AddCqs(this IServiceCollection services)
            {
                services.AddTransient<IQueryProcessor, QueryProcessor>();
                services.AddTransient<ICommandDispatcher, CommandDispatcher>();
                return services;
            }
        }
}
