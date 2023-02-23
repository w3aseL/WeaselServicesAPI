using WeaselServicesAPI.Models;

namespace WeaselServicesAPI.Helpers
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
