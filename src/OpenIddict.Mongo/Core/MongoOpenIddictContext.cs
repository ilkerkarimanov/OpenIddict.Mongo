using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIddict.Mongo.Core
{
    public class MongoOpenIddictContext : IMongoOpenIddictContext
    {
        private IMongoClient _client { get; set; }
        private IMongoDatabase _database { get; set; }

        public MongoOpenIddictContext(IMongoOpenIddictConnection conn)
        {
            this.Create(conn);
        }
        private void Create(IMongoOpenIddictConnection mongoConn)
        {
            var url = new MongoUrl(mongoConn.connectionString());
            this._client = new MongoClient(url.Url);
            this._database = this._client.GetDatabase(url.DatabaseName);
        }

        IMongoDatabase IMongoOpenIddictContext.Database
        {
            get
            {
                return _database;
            }
        }
    }
}
