using MimeKit;

namespace EmailService.Models
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("", x)));
            Subject = subject;
            Content = content;
        }
    }

    public class ModeledMessage<T>
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public T Model { get; set; }
        public ModeledMessage(IEnumerable<string> to, string subject, T model)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("", x)));
            Subject = subject;
            Model = model;
        }
    }
}
