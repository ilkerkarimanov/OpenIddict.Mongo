using IDDD.App.Cqs.Commands.Users;
using IDDD.Core.Cqs;
using IDDD.Core.Cqs.Command;
using IDDD.Core.Domain;
using IDDD.Domain.Membership;
using IDDD.Domain.Membership.Clients;
using IDDD.Domain.Messaging;
using IDDD.Core;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace IDDD.App.Cqs.CommandHandlers.Users
{
    public class ResetPasswordHandler :
        IAsyncCommandHandler<ForgotPasswordCommand, Result>,
        IAsyncCommandHandler<ResetPasswordCommand, Result>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IClientFinder _clientFinder;
        public ResetPasswordHandler(UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            IClientFinder clientFinder)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _clientFinder = clientFinder;
        }
        public async Task<Result> HandleAsync(ForgotPasswordCommand command)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if(user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                throw new FailureResult("Could not reset user password");
            }
            var client = await GetUserClient(command.ClientId);
            SendResetPasswordRequest(user, client);
            return await Task.FromResult(Result.Ok());
        }

        public async Task<Result> HandleAsync(ResetPasswordCommand command)
        {
            var user = await _userManager.FindByNameAsync(command.UserName);
            if (user == null)
            {
                return await Task.FromResult(Result.Fail("Could not reset password"));
            }
            var result = await _userManager.ResetPasswordAsync(user, command.ConfirmationToken, command.NewPassword);
            if (!result.Succeeded)
            {
                var message = "Could not reset password";
                return await Task.FromResult(Result.Fail(message));
            }
            return Result.Ok();
        }

        private async Task<Client> GetUserClient(string clientKey)
        {
            var result = await _clientFinder.GetByClientId(clientKey);
            if (result.HasNoValue)
            {
                var message = $"Client '{clientKey}' is not registered in the system.";
                throw new FailureResult(message);
            }
            return result.Value;
        }

        private async void SendResetPasswordRequest(IdentityUser user, Client client)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = UrlEncoder.Default.Encode(code);
            var callbackUrl = new Uri(
                $"{client.ConfirmationUri.TrimEnd('/')}?userName={user.UserName}&confirmationToken={code}&confirmationMode=resetPassword");
            await _emailSender
                .SendEmailAsync(user.Email, "Reset your password",
                $"Please reset your account password by clicking this link: {callbackUrl}");
        }


    }
}
