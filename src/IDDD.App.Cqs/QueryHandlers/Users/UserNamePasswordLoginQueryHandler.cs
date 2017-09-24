using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using IDDD.Core.Cqs.Query;
using IDDD.Domain.Membership;
using System;
using IDDD.App.Cqs.Queries.Users;
using IDDD.App.Cqs.QueryResult.Users;

namespace IDDD.App.Cqs.QueryHandlers.Users
{
    public class UserNamePasswordLoginQueryHandler :
        IHandleQueryAsync<UserNamePasswordLoginQuery, LoginResult>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserNamePasswordLoginQueryHandler(UserManager<IdentityUser> userManager)
              
        {
            _userManager = userManager;
        }

        public async Task<LoginResult> ExecuteAsync(UserNamePasswordLoginQuery query)
        {
            var user = await _userManager.FindByNameAsync(query.UserName);
            if (user == null)
            {
                return new LoginResult(false);
            }

            var valid = await _userManager.CheckPasswordAsync(user, query.Password);

            return valid
                ? new LoginResult(true, Guid.Parse(user.Id.Id), user.UserName)
                : new LoginResult(false);
        }
    }
}