using DataAccessLayer.Models;

namespace WeaselServicesAPI
{
    public class LinkModel
    {
        public int LinkId { get; set; }
        public string LinkName { get; set; }
        public string LinkUrl { get; set; }
        public string LogoUrl { get; set; }
        public string LogoAlt { get; set; }

        public LinkModel() { }

        public Link ToDBEntry()
        {
            return new Link()
            {
                LinkName = LinkName,
                LinkUrl = LinkUrl,
                LogoUrl = LogoUrl,
                LogoAlt = LogoAlt
            };
        }
    }
}
