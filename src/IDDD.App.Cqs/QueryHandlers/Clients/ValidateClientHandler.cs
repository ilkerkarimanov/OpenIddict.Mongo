using System.Threading.Tasks;
using IDDD.Core.Cqs.Query;
using IDDD.Domain.Membership.Clients;
using IDDD.App.Cqs.QueryResult.Clients;
using IDDD.App.Cqs.Queries.Clients;
using System.Threading;
using System.Linq;

namespace IDDD.App.Cqs.QueryHandlers.Clients
{
    public class ValidateClientHandler :
        IHandleQueryAsync<ValidateClientQuery, ValidateClientResult>
    {
        private readonly IClientFinder _clientFinder;

        public ValidateClientHandler(IClientFinder clientFinder)
        {
            _clientFinder = clientFinder;
        }

        public async Task<ValidateClientResult> ExecuteAsync(ValidateClientQuery query)
        {
            return await GetResult(query);
        }

        private async Task<ValidateClientResult> GetResult(ValidateClientQuery query)
        {
            var result = await _clientFinder.GetByClientId(query.ClientId);
            if (result.HasNoValue)
            {
                return ValidateClientResult.Failed($"Client '{query.ClientId}' is not registered in the system.");
            }
            var client = result.Value;
            if (!client.Active)
            {
                return ValidateClientResult.Failed("Client is inactive.");
            }

            return new ValidateClientResult(true, client.Id.Id, client.Name, client.AllowedOrigin, client.RedirectUri);
        }
    }
}