using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using IDDD.Core.Cqs.Query;
using IDDD.Domain.Membership;
using System;
using IDDD.App.Cqs.Queries.Users;
using IDDD.App.Cqs.QueryResult.Users;
using IDDD.Core.Domain;
using IDDD.Core;

namespace IDDD.App.Cqs.QueryHandlers.Users
{
    public class UserInfoQueryHandler : 
        IHandleQueryAsync<UserInfoQuery, UserInfoResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IQueryProcessor _queryProcessor;

        public UserInfoQueryHandler(UserManager<IdentityUser> userManager,
            IQueryProcessor queryProcessor)
        {
            _userManager = userManager;
            _queryProcessor = queryProcessor;
        }

        public async Task<UserInfoResult> ExecuteAsync(UserInfoQuery query)
        {
            IdentityUser result = await _userManager.FindByIdAsync(query.Id.ToString());
            if (result == null)
                return null;
            var userInfo = Map(result);
            return await Task.FromResult(userInfo);
        }

        private UserInfoResult Map(IdentityUser result)
        {
            return new UserInfoResult
            {
                Id = Guid.Parse(result.Id.Id),
                UserName = result.UserName,
                Email = result.Email
            };
        }
    }
}