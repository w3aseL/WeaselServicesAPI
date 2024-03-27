using EmailService.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailConfig;
        private readonly IEmailRenderer _renderer;

        public EmailSender(EmailSettings emailConfig, IEmailRenderer renderer)
        {
            _emailConfig = emailConfig;
            _renderer = renderer;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        public async Task SendEmailWithModel<T>(ModeledMessage<T> message)
        {
            var emailMessage = await CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.Name, _emailConfig.FromEmail));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }

        private async Task<MimeMessage> CreateEmailMessage<T>(ModeledMessage<T> message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.Name, _emailConfig.FromEmail));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            var content = await _renderer.Render(message.Model);
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = content };
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.Host, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.FromEmail, _emailConfig.Password);
                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
