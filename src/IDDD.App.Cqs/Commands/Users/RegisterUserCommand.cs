using IDDD.Core.Cqs.Command;

namespace IDDD.App.Cqs.Commands.Users
{
    public class RegisterUserCommand : ICommand
    {
        public string Email { get;  }
        public string ClientKey { get; }

        public RegisterUserCommand(string email, string clientKey)
        {
            Email = email;
            ClientKey = clientKey;
        }
    }
}
