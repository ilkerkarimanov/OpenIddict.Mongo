using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIddict.Mongo.Core
{
    public class MongoOpenIddictConnection : IMongoOpenIddictConnection
    {
        private string _connectionString {get; set;}

        public MongoOpenIddictConnection()
        {
            _connectionString = "mongodb://localhost:27017/openiddict-store";
        }
        public string connectionString()
        {
            return _connectionString;
        }
    }
}
