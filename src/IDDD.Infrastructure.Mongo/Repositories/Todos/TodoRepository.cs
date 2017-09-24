using System.Threading.Tasks;
using System.Threading;
using MongoDB.Driver;
using IDDD.Domain.Todos;

namespace IDDD.Infrastructure.Mongo.Repositories.Todos
{
    public class TodoRepository : MongoRepositoryBase, ITodoRepository
    {
        public TodoRepository(IMongoContext dbContext) : base(dbContext)
        {
        }


        public async Task Create(Todo entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await _dbContext.Todos.InsertOneAsync(entity);
        }

        public async Task Delete(Todo entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<Todo>.Filter.Eq(s => s.Id, entity.Id);
            await _dbContext.Todos.DeleteOneAsync(filter);
        }

        public async Task Update(Todo entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filter = Builders<Todo>.Filter.Eq(s => s.Id, entity.Id);
            
            await _dbContext.Todos.ReplaceOneAsync(filter, entity);
        }

    }
}
