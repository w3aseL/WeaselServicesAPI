﻿using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
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

        public List<Education> GetEducations()
        {
            return _ctx.Educations.ToList();
        }

        public Education GetEducation(string id)
        {
            var edu = _ctx.Educations.FirstOrDefault(e => e.Id.ToString() == id);

            if (edu is null)
                throw new ArgumentNullException("There is no education object that exists with that identifier!");

            return edu;
        }

        public Education CreateEducation(string schoolName, string schoolType, string rewardType, DateTime graduationDate, decimal gpa, string? major)
        {
            var edu = new Education()
            {
                SchoolName = schoolName,
                SchoolType = schoolType,
                RewardType = rewardType,
                GraduationDate = graduationDate,
                Gpa = gpa,
                Major = major
            };

            _ctx.Educations.Add(edu);
            _ctx.SaveChanges();

            return edu;
        }

        public async Task SetLogo(string id, IFormFile logoFile)
        {
            var edu = _ctx.Educations.FirstOrDefault(e => e.Id.ToString() == id);

            if (edu is null)
                throw new ArgumentNullException("There is no education object that exists with that identifier!");

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

                edu.ImageId = image.Id;
                _ctx.SaveChanges();

                tx.Commit();
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
