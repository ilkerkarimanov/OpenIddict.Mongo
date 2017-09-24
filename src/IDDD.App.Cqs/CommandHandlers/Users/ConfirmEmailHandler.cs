using IDDD.App.Cqs.Commands.Users;
using IDDD.Core.Cqs;
using IDDD.Core.Cqs.Command;
using IDDD.Domain.Membership;
using IDDD.Core;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace IDDD.App.Cqs.CommandHandlers.Users
{
    public class ConfirmEmailHandler :
        IAsyncCommandHandler<ConfirmEmailCommand, Result>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmEmailHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result> HandleAsync(ConfirmEmailCommand command)
        {
            var user = await GetUserByName(command.UserName);
            if(user == null)
            {
                return await Task.FromResult(Result.Fail("Could not confirm user email"));

            }
            var result = await _userManager.ConfirmEmailAsync(user, command.ConfirmationToken);
            if (!result.Succeeded)
            {
                var message = "Could not confirm user email";
                return await Task.FromResult(Result.Fail(message));
            }
            return await Task.FromResult(Result.Ok());
        }

        private async Task<IdentityUser> GetUserByName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }
    }
}
