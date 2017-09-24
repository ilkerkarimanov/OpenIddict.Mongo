using System;

namespace IDDD.App.Cqs.QueryResult.Users
{
    public class UserInfoResult
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        //public UserProfileInfoResult ProfileInfo { get; set; }
    }
}