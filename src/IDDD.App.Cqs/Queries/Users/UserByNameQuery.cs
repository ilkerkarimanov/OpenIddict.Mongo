using IDDD.App.Cqs.QueryResult.Users;
using IDDD.Core.Cqs.Query;

namespace IDDD.App.Cqs.Queries.Users
{
    public class UserByNameQuery : IQuery<UserResult>
    {
        public string UserName { get; }

        public UserByNameQuery(string userName)
        {
            UserName = userName;
        }
    }
}