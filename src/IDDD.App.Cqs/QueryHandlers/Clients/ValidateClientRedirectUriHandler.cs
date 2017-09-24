using IDDD.Core.Cqs.Query;
using System.Threading.Tasks;
using IDDD.Domain.Membership.Clients;
using IDDD.App.Cqs.Queries.Clients;
using System.Threading;
using IDDD.Core.Cqs;
using IDDD.Core;

namespace IDDD.App.Cqs.QueryHandlers.Clients
{
    public class ValidateClientRedirectUriHandler :
        IHandleQueryAsync<ValidateClientRedirectUriQuery, Result>
    {
        private readonly IClientFinder _clientFinder;

        public ValidateClientRedirectUriHandler(IClientFinder clientFinder)
        {
            _clientFinder = clientFinder;
        }

        public async Task<Result> ExecuteAsync(ValidateClientRedirectUriQuery query)
        {
            return await GetResult(query);
        }

        private async Task<Result> GetResult(ValidateClientRedirectUriQuery query)
        {
            Maybe<Client> result = await _clientFinder.
                GetById(new ClientId(query.ClientId), default(CancellationToken));
            
            if (result.HasNoValue)
            {
                return Result.Fail($"Client '{query.ClientId}' is not registered in the system.");
            }
            var client = result.Value;
            if (client.RedirectUri != "*" && client.RedirectUri != query.RedirectUri)
            {
                return Result.Fail($"Invalid redirect uri '{query.RedirectUri} for client '{query.ClientId}'");
            }

            if (!client.Active)
            {
                return Result.Fail("Client is inactive.");
            }

            return Result.Ok();
        }
    }
}