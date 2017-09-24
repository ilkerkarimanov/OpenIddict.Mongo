using IDDD.Domain.Membership;
using IDDD.Domain.Membership.Clients;
using IDDD.Domain.Todos;
using MongoDB.Driver;

namespace IDDD.Infrastructure.Mongo
{
    public interface IMongoContext {
       
        IMongoCollection<Todo> Todos { get; }
        IMongoCollection<IdentityUser> Users { get; }
        IMongoCollection<IdentityRole> Roles { get; }
        IMongoDatabase Database { get; }

    }
    
}
