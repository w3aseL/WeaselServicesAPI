using DataAccessLayer;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioLibrary.Services
{
    public class ImageService
    {
        private ServicesAPIContext _ctx;
        private S3Service _s3Service;

        public ImageService(ServicesAPIContext ctx, S3Service s3Service)
        {
            _ctx = ctx;
            _s3Service = s3Service;
        }

        public List<Image> GetImages()
        {
            return _ctx.Images.ToList();
        }

        public Image GetImage(string id)
        {
            var image = _ctx.Images.FirstOrDefault(i => i.Id.ToString() == id);

            if (image is null)
                throw new ArgumentNullException("There is no image that exists with that identifier!");

            return image;
        }

        public async Task DeleteImage(string id)
        {
            var image = _ctx.Images.FirstOrDefault(i => i.Id.ToString() == id);

            if (image is null)
                throw new ArgumentNullException("There is no image that exists with that identifier!");

            await _s3Service.DeleteFile(image.Key);

            _ctx.Images.Remove(image);
            _ctx.SaveChanges();
        }
    }
}
