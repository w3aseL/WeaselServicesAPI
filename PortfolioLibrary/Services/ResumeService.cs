using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioLibrary.Services
{
    public class ResumeService
    {
        private ServicesAPIContext _ctx;
        private S3Service _s3Service;

        public ResumeService(ServicesAPIContext ctx, S3Service s3Service)
        {
            _ctx = ctx;
            _s3Service = s3Service;
        }

        public List<Resume> GetResumes()
        {
            return _ctx.Resumes.ToList();
        }

        public async Task<Resume> UploadResume(IFormFile file)
        {
            using var tx = _ctx.Database.BeginTransaction();

            try
            {
                var folder = $"resumes";
                var key = $"{ folder }/{ file.FileName }";

                await _s3Service.UploadFile(folder, file);

                var resume = new Resume()
                {
                    Key = key,
                    FileName = file.FileName,
                    CreationDate = SqlDateTime.MinValue.Value,
                    Url = $"{ _s3Service.GetSiteUrlFromBucketName() }{ key }"
                };

                _ctx.Resumes.Add(resume);
                _ctx.SaveChanges();

                tx.Commit();

                return resume;
            }
            catch(Exception)
            {
                tx.Rollback();
                throw;
            }
        }

        public Resume UpdateResumeDate(string id, DateTime creationDate)
        {
            var resume = _ctx.Resumes.FirstOrDefault(r => r.Id.ToString() == id);

            if (resume is null)
                throw new ArgumentNullException("There is no resume that exists with that identifier!");

            resume.CreationDate = creationDate;

            _ctx.SaveChanges();

            return resume;
        }

        public async Task DeleteResume(string id)
        {
            var resume = _ctx.Resumes.FirstOrDefault(r => r.Id.ToString() == id);

            if (resume is null)
                throw new ArgumentNullException("There is no resume that exists with that identifier!");

            await _s3Service.DeleteFile(resume.Key);

            _ctx.Resumes.Remove(resume);
            _ctx.SaveChanges();
        }
    }
}
