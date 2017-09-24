using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using IDDD.Core.Cqs.Query;
using IDDD.Domain.Membership;
using System;
using IDDD.App.Cqs.Queries.Users;
using IDDD.App.Cqs.QueryResult.Users;

namespace IDDD.App.Cqs.QueryHandlers.Users
{
    public class UserByNameQueryHandler : 
        IHandleQueryAsync<UserByNameQuery, UserResult>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserByNameQueryHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserResult> ExecuteAsync(UserByNameQuery query)
        {
            var result = await _userManager.FindByNameAsync(query.UserName);
            return result != null ? Map(result) : null;
        }

        private UserResult Map(IdentityUser result)
        {
            return new UserResult
            {
                Id = Guid.Parse(result.Id.Id),
                UserName = result.UserName
            };
        }
    }
}