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
    public class EducationService
    {
        private ServicesAPIContext _ctx;
        private S3Service _s3Service;

        public EducationService(ServicesAPIContext ctx, S3Service s3Service)
        {
            _ctx = ctx;
            _s3Service = s3Service;
        }

        public List<EducationViewModel> GetEducations()
        {
            return _ctx.Educations
                .Include(e => e.Image)
                .ToList()
                .Select(e => new EducationViewModel(e))
                .ToList();
        }

        public List<Education> GetEducationsNoModel()
        {
            return _ctx.Educations
                .Include(e => e.Image)
                .ToList();
        }

        public EducationViewModel GetEducation(string id)
        {
            var guid = Guid.NewGuid();

            if (Guid.TryParse(id, out Guid outGuid))
                guid = outGuid;

            var edu = _ctx.Educations
                .Include(e => e.Image)
                .FirstOrDefault(e => e.Id == guid);

            if (edu is null)
                throw new ArgumentNullException("There is no education object that exists with that identifier!");

            return new EducationViewModel(edu);
        }

        public EducationViewModel CreateEducation(string schoolName, string schoolType, string rewardType, DateTime graduationDate, decimal gpa, string schoolUrl, string? major, string? imageId)
        {
            using var tx = _ctx.Database.BeginTransaction();        // y i did this?

            try
            {
                Image img = null;

                if (imageId != null)
                {
                    var guid = Guid.NewGuid();

                    if (Guid.TryParse(imageId, out Guid outGuid))
                        guid = outGuid;

                    img = _ctx.Images.FirstOrDefault(img => img.Id == guid);
                }

                var edu = new Education()
                {
                    SchoolName = schoolName,
                    SchoolType = schoolType,
                    RewardType = rewardType,
                    GraduationDate = graduationDate,
                    Gpa = gpa,
                    SchoolUrl = schoolUrl,
                    Major = major
                };

                if (img != null) edu.ImageId = img.Id;

                _ctx.Educations.Add(edu);
                _ctx.SaveChanges();

                tx.Commit();

                return new EducationViewModel(edu);
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<Image> UploadLogo(IFormFile logoFile)
        {
            using var tx = _ctx.Database.BeginTransaction();

            try
            {
                var folder = $"education/logo";

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

        public void DeleteEducation(string id)
        {
            var edu = _ctx.Educations.FirstOrDefault(e => e.Id.ToString() == id);

            if (edu is null)
                throw new ArgumentNullException("There is no education object that exists with that identifier!");

            _ctx.Educations.Remove(edu);
            _ctx.SaveChanges();
        }
    }
}
