using System;
using IDDD.Core.Cqs.Query;
using IDDD.App.Cqs.QueryResult.Users;

namespace IDDD.App.Cqs.Queries.Users
{
    public class UserInfoQuery : IQuery<UserInfoResult>
    {
        public Guid Id { get; }

        public UserInfoQuery(Guid id)
        {
            Id = id;
        }
    }
}