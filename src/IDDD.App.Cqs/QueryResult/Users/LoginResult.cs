using IDDD.Core.Cqs;
using IDDD.Core;
using System;

namespace IDDD.App.Cqs.QueryResult.Users
{
    public class LoginResult : Result
    {
        public Guid UserId { get; }
        public string UserName { get; }

        public LoginResult()
        {
        }

        public LoginResult(bool success, Guid userId = default(Guid), string userName = null) : base(success)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
