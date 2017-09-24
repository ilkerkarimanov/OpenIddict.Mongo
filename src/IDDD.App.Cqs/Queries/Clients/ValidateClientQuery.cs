using IDDD.App.Cqs.QueryResult.Clients;
using IDDD.Core.Cqs.Query;

namespace IDDD.App.Cqs.Queries.Clients
{
    public class ValidateClientQuery : IQuery<ValidateClientResult>
    {
        public string ClientId { get; }
        public string ClientSecret { get; }

        public ValidateClientQuery(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}