using System.Threading.Tasks;
using System.Threading;
using IDDD.Domain.Membership.Clients;
using OpenIddict.Models;
using OpenIddict.Core;

namespace IDDD.Infrastructure.Mongo.Repositories
{
    public class ClientFinder : MongoRepositoryBase, IClientFinder
    {
        private readonly OpenIddictApplicationManager<OpenIddictApplication> _applicationManager;

        public ClientFinder(IMongoContext dbContext,
            OpenIddictApplicationManager<OpenIddictApplication> applicationManager
            ) : base(dbContext)
        {
            _applicationManager = applicationManager;
        }

        public async Task<Maybe<Client>> GetById(ClientId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await _applicationManager.FindByIdAsync(id.Id, cancellationToken);

            return await Task.Factory.StartNew(() =>
            {
                var client = new Client()
                {
                    Id = new ClientId(result.Id),
                    Active = result.Active,
                    AllowedOrigin = result.AllowedOrigin,
                    ApplicationType = result.Type,
                    Key = result.ClientId,
                    Name = result.DisplayName,
                    RedirectUri = result.RedirectUri,
                    Secret = result.ClientSecret,
                    LogoutRedirectUri = result.LogoutRedirectUri
                };
                return client;
            }, cancellationToken);
        }

        public async Task<Maybe<Client>> GetByClientId(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await _applicationManager.FindByClientIdAsync(key, cancellationToken);

            return await Task.Factory.StartNew(() =>
            {
                var client = new Client()
                {
                    Id = new ClientId(result.Id),
                    Active = result.Active,
                    AllowedOrigin = result.AllowedOrigin,
                    ApplicationType = result.Type,
                    Key = result.ClientId,
                    Name = result.DisplayName,
                    RedirectUri = result.RedirectUri,
                    Secret = result.ClientSecret,
                    LogoutRedirectUri = result.LogoutRedirectUri
                };
                return client;
            }, cancellationToken);
        }
    }
}
