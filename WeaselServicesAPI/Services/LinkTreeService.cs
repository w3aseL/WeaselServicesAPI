using DataAccessLayer;
using DataAccessLayer.Models;
using WeaselServicesAPI.Helpers.Interfaces;

namespace WeaselServicesAPI.Services
{
    public class LinkTreeService
    {
        private readonly ServicesAPIContext _ctx;

        public LinkTreeService(ServicesAPIContext ctx)
        {
            _ctx = ctx;
        }

        public List<Link> GetAllLinks()
        {
            return _ctx.Links.OrderBy(l => l.OrderNumber).ToList();
        }

        public Link CreateLink(LinkModel model)
        {
            var link = model.ToDBEntry();
            link.OrderNumber = _ctx.Links.Count() + 1;

            _ctx.Add(link);
            _ctx.SaveChanges();

            return link;
        }

        public Link EditLink(LinkModel model)
        {
            var link = _ctx.Links.FirstOrDefault(l => l.Id == model.LinkId);

            if (link == null)
                throw new ArgumentException($"Could not find link with the identifier \"{ model.LinkId }\".");

            UpdateLink(link, model);
            _ctx.SaveChanges();

            return link;
        }

        public void DeleteLink(int linkId)
        {
            var link = _ctx.Links.FirstOrDefault(l => l.Id ==  linkId);

            if (link == null)
                throw new ArgumentException($"Could not find link with the identifier \"{ linkId }\".");

            var orderNumber = link.OrderNumber;

            _ctx.Links.Remove(link);

            var links = _ctx.Links.ToList();

            foreach (var l in links)
            {
                if (l.OrderNumber > orderNumber) l.OrderNumber -= 1;
            }

            _ctx.SaveChanges();
        }

        public void ChangeLinkOrder(int linkId, bool increase)
        {
            var link = _ctx.Links.FirstOrDefault(l => l.Id == linkId);

            if (link == null)
                throw new ArgumentException($"Could not find link with the identifier \"{linkId}\".");

            var orderNumber = link.OrderNumber;

            if (!increase)
            {
                var otherLink = _ctx.Links.FirstOrDefault(l => l.OrderNumber == orderNumber + 1);

                if (otherLink != null) otherLink.OrderNumber -= 1;
                link.OrderNumber += 1;
            }
            else
            {
                var otherLink = _ctx.Links.FirstOrDefault(l => l.OrderNumber == orderNumber - 1);

                if (otherLink != null) otherLink.OrderNumber += 1;
                link.OrderNumber -= 1;
            }

            _ctx.SaveChanges();
        }

        private void UpdateLink(Link l, LinkModel model)
        {
            if (!string.IsNullOrEmpty(model.LinkName) && l.LinkName != model.LinkName) l.LinkName = model.LinkName;
            if (!string.IsNullOrEmpty(model.LinkUrl) && l.LinkUrl != model.LinkUrl) l.LinkUrl = model.LinkUrl;
            if (!string.IsNullOrEmpty(model.LogoUrl) && l.LogoUrl != model.LogoUrl) l.LogoUrl = model.LogoUrl;
            if (!string.IsNullOrEmpty(model.LogoAlt) && l.LogoAlt != model.LogoAlt) l.LogoAlt = model.LogoAlt;
        }
    }
}
