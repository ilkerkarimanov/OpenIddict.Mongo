using Microsoft.Extensions.Configuration;
using OpenIddict.Mongo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expandable.Infrastructure.Membership.OpenIddict
{
    public class IDDDOpenIddictConnection : IMongoOpenIddictConnection
    {
        private IConfiguration _configuration;

        public IDDDOpenIddictConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string connectionString()
        {
            return expandableOpenIddictStore();
        }

        private string expandableOpenIddictStore(string name = "DDDMongoStore")
        {
            if (_configuration == null) throw new ArgumentNullException(nameof(_configuration));

            return _configuration[$"connectionStrings:{name}"];
        }
    }
}
