using IDDD.App.Cqs.QueryResult.Users;
using IDDD.Core.Cqs.Query;

namespace IDDD.App.Cqs.Queries.Users
{
    public class UserNamePasswordLoginQuery : IQuery<LoginResult>
    {
        public string UserName { get; }
        public string Password { get; }

        public UserNamePasswordLoginQuery(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}