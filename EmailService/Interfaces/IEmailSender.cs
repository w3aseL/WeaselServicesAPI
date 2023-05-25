using EmailService.Models;

namespace EmailService
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
