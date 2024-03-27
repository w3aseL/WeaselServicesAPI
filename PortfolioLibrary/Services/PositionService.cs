using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioLibrary.Services
{
    public class PositionService
    {
        private ServicesAPIContext _ctx;
        private S3Service _s3Service;

        public PositionService(ServicesAPIContext ctx, S3Service s3Service)
        {
            _ctx = ctx;
            _s3Service = s3Service;
        }

        public List<Position> GetPositions()
        {
            return _ctx.Positions.ToList();
        }

        public Position GetPosition(string id)
        {
            var guid = Guid.NewGuid();

            if (Guid.TryParse(id, out Guid outGuid))
                guid = outGuid;

            var pos = _ctx.Positions.FirstOrDefault(p => p.Id == guid);

            if (pos is null)
                throw new ArgumentNullException("There is no position object that exists with that identifier!");

            return pos;
        }

        public Position CreatePosition(string jobTitle, string companyName, string companyUrl, string description, DateTime startDate, DateTime? endDate)
        {
            var pos = new Position
            {
                JobTitle = jobTitle,
                CompanyName = companyName,
                CompanyUrl = companyUrl,
                Description = description,
                StartDate = startDate,
                EndDate = endDate
            };

            _ctx.Positions.Add(pos);
            _ctx.SaveChanges();

            return pos;
        }

        public async Task SetLogo(string id, IFormFile logoFile)
        {
            var guid = Guid.NewGuid();

            if (Guid.TryParse(id, out Guid outGuid))
                guid = outGuid;

            var pos = _ctx.Positions.FirstOrDefault(e => e.Id == guid);

            if (pos is null)
                throw new ArgumentNullException("There is no position object that exists with that identifier!");

            using var tx = _ctx.Database.BeginTransaction();

            try
            {
                var folder = $"positions/logo";

                await _s3Service.UploadFile(folder, logoFile);

                var image = new Image()
                {
                    Key = $"{folder}/{logoFile.FileName}",
                    FileName = logoFile.FileName,
                    Url = $"{_s3Service.GetSiteUrlFromBucketName()}{folder}/{logoFile.FileName}"
                };

                _ctx.Images.Add(image);
                _ctx.SaveChanges();

                pos.ImageId = image.Id;
                _ctx.SaveChanges();

                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
        }

        public void DeletePosition(string id)
        {
            var guid = Guid.NewGuid();

            if (Guid.TryParse(id, out Guid outGuid))
                guid = outGuid;

            var pos = _ctx.Positions.FirstOrDefault(p => p.Id == guid);

            if (pos is null)
                throw new ArgumentNullException("There is no position object that exists with that identifier!");

            _ctx.Positions.Remove(pos);
            _ctx.SaveChanges();
        }
    }
}
