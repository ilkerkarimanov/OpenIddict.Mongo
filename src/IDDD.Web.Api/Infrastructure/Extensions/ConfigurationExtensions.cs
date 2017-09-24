using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDDD.App
{
    public static class ConfigurationExtensions
    {
        public static string ApiHostName(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["Auth:Api:HostName"];
        }

        public static string AuthorityHostName(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["Auth:Identity:HostName"];
        }
    }
}
