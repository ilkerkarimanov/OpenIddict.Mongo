using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using IDDD.Core.Cqs.Command;
using IDDD.Domain.Membership;
using IDDD.App.Cqs.Commands.Users;
using System;
using System.Text.Encodings.Web;
using IDDD.Domain.Membership.Clients;
using System.Linq;
using IDDD.Core;
using IDDD.Domain.Messaging;

namespace IDDD.App.Cqs.CommandHandlers.Users
{
    public class RegisterUserHandler :
        IAsyncCommandHandler<RegisterUserCommand, Result>,
        IAsyncCommandHandler<CreatePasswordCommand, Result>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IClientFinder _clientFinder;

        public RegisterUserHandler(UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            IClientFinder clientFinder)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _clientFinder = clientFinder;
        }

        public async Task<Result> HandleAsync(RegisterUserCommand command)
        {
            var client = GetUserClient(command.ClientKey);
            var user = new IdentityUser()
            {
                UserName = command.Email,
                Email = command.Email
            };
            var result = await _userManager.CreateAsync(user);

            return result.ToCommandResult();
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

        private async void SendEmailConfirmation(IdentityUser user, Client client)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = UrlEncoder.Default.Encode(code);
            var callbackUrl = new Uri(
                $"{client.ConfirmationUri.TrimEnd('/')}?userName={user.UserName}&confirmationToken={code}&confirmationMode=confirmEmail");
            await _emailSender
                .SendEmailAsync(user.Email, "Confirm your account",
                $"Please confirm your account by clicking this link: {callbackUrl}");
        }

        public async Task<Result> HandleAsync(CreatePasswordCommand command)
        {
            var user = await _userManager.FindByNameAsync(command.UserName);
            if(user == null)
                throw new FailureResult("Could not change user password.");
            var result = await _userManager.AddPasswordAsync(user, command.Password);
            if(result.Errors.Count() > 0){
                throw new FailureResult(result.Errors.First().Description);
                    }
            return result.ToCommandResult();
        }
    }
}