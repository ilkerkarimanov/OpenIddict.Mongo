using IDDD.Core.Cqs.Command;

namespace IDDD.App.Cqs.Commands.Users
{
    public class ExternalLoginCommand : ICommand
    {
        public string ExternalType { get; }
        public string UserName { get; }
        public string ExternalIdentifier { get; }
        public string Email { get; }

        public ExternalLoginCommand(string externalType, string userName, string externalIdentifier, string email)
        {
            ExternalType = externalType;
            UserName = userName;
            ExternalIdentifier = externalIdentifier;
            Email = email;
        }
    }
}