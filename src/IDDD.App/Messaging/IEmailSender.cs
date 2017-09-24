using System.Threading.Tasks;

namespace IDDD.Domain.Messaging
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
