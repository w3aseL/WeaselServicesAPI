using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortfolioLibrary.Models;
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
            return _ctx.Positions
                .Include(p => p.Image)
                .ToList();
        }

        public List<PositionViewModel> GetPositionsWithModel()
        {
            return _ctx.Positions
                .Include(p => p.Image)
                .ToList()
                .Select(p => new PositionViewModel(p))
                .ToList();
        }

        public PositionViewModel GetPosition(string id)
        {
            var guid = Guid.NewGuid();

            if (Guid.TryParse(id, out Guid outGuid))
                guid = outGuid;

            var pos = _ctx.Positions
                .Include(p => p.Image)
                .FirstOrDefault(p => p.Id == guid);

            if (pos is null)
                throw new ArgumentNullException("There is no position object that exists with that identifier!");

            return new PositionViewModel(pos);
        }

        public PositionViewModel CreatePosition(string jobTitle, string companyName, string companyUrl, string description, DateTime startDate, DateTime? endDate, string? imageId)
        {
            var img = GetImage(imageId);

            var pos = new Position
            {
                JobTitle = jobTitle,
                CompanyName = companyName,
                CompanyUrl = companyUrl,
                Description = description,
                StartDate = startDate,
                EndDate = endDate
            };

            if (img != null) pos.ImageId = img.Id;

            _ctx.Positions.Add(pos);
            _ctx.SaveChanges();

            return new PositionViewModel(pos);
        }

        public PositionViewModel UpdatePosition(string id, string jobTitle, string companyName, string companyUrl, string description, DateTime startDate, DateTime? endDate, string? imageId)
        {
            var img = GetImage(imageId);

            var guid = Guid.NewGuid();

            if (Guid.TryParse(id, out Guid outGuid))
                guid = outGuid;

            var pos = _ctx.Positions
                .Include(p => p.Image)
                .FirstOrDefault(p => p.Id == guid);

            if (pos is null)
                throw new ArgumentNullException("There is no position object that exists with that identifier!");

            if (pos.JobTitle != jobTitle) pos.JobTitle = jobTitle;
            if (pos.CompanyName != companyName) pos.CompanyName = companyName;
            if (pos.CompanyUrl != companyUrl) pos.CompanyUrl = companyUrl;
            if (pos.Description != description) pos.Description = description;
            if (pos.StartDate != startDate) pos.StartDate = startDate;
            if (pos.EndDate != endDate) pos.EndDate = endDate;
            if (img != null && pos.ImageId != img.Id) pos.ImageId = img.Id;

            _ctx.SaveChanges();

            return new PositionViewModel(pos);
        }

        private Image GetImage(string? id)
        {
            Image img = null;

            if (id != null)
            {
                var guid = Guid.NewGuid();

                if (Guid.TryParse(id, out Guid outGuid))
                    guid = outGuid;

                img = _ctx.Images.FirstOrDefault(img => img.Id == guid);
            }

            return img;
        }

        public async Task<Image> UploadLogo(IFormFile logoFile)
        {
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

                tx.Commit();

                return image;
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
