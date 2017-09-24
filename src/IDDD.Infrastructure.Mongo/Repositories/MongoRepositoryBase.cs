using System;


namespace IDDD.Infrastructure.Mongo.Repositories
{
    public abstract class MongoRepositoryBase
    {
		protected readonly IMongoContext _dbContext;

		public MongoRepositoryBase(IMongoContext dbContext)
		{
			if (dbContext == null) throw new ArgumentNullException("MongoDbContext");
			_dbContext = dbContext;
		}
	}
}
