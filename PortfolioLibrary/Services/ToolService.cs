using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioLibrary.Services
{
    public class ToolService
    {
        private ServicesAPIContext _ctx;
        private S3Service _s3Service;

        public ToolService(ServicesAPIContext ctx, S3Service s3Service)
        {
            _ctx = ctx;
            _s3Service = s3Service;
        }

        public List<Tool> GetTools()
        {
            return _ctx.Tools
                .Include(t => t.Image)
                .Include(t => t.Category)
                .ToList();
        }

        public Tool GetTool(int id)
        {
            var tool = _ctx.Tools
                .Include(t => t.Image)
                .Include(t => t.Category)
                .FirstOrDefault(t => t.Id == id);

            if (tool is null)
                throw new ArgumentException("There is no tool object that exists with that identifier!");

            return tool;
        }

        public Tool CreateTool(string name, string url, string description, string category, string? imageId)
        {
            var image = GetImage(imageId);
            var categoryObj = GetCategory(category);

            if (_ctx.Tools.Any(t => t.Name == name))
                throw new ArgumentException($"There is a tool that already exists with the name \"{name}\"!");

            var tool = new Tool()
            {
                Name = name,
                Url = url,
                Description = description
            };

            if (categoryObj != null) tool.CategoryId = categoryObj.Id;
            if (image != null) tool.ImageId = image.Id;

            _ctx.Tools.Add(tool);
            _ctx.SaveChanges();

            return tool;
        }

        public async Task<Image> UploadLogo(IFormFile logoFile)
        {
            using var tx = _ctx.Database.BeginTransaction();

            try
            {
                var folder = $"tools/logo";

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

        public Tool UpdateTool(int id, string name, string description, string url, string category, string? imageId)
        {
            var categoryObj = GetCategory(category);
            var image = GetImage(imageId);

            var tool = _ctx.Tools
                .FirstOrDefault(t => t.Id == id);

            if (tool is null)
                throw new ArgumentException("There is no tool object that exists with that identifier!");

            if (_ctx.Tools.Any(t => t.Name == name && t.Id != id))
                throw new ArgumentException($"The tool cannot be renamed to \"{name}\", as a tool with that name already exists!");

            if (tool.Name != name) tool.Name = name;
            if (tool.Description !=  description) tool.Description = description;
            if (tool.Url != url) tool.Url = url; 
            if (tool.CategoryId != categoryObj.Id) tool.CategoryId = categoryObj.Id;
            if (tool.ImageId != image.Id) tool.ImageId = image.Id;

            _ctx.SaveChanges();

            return tool;
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

        private Category GetCategory(string name)
        {
            var obj = _ctx.Categories.FirstOrDefault(c => c.Name == name);

            if (obj is null && name != null)
            {
                obj = new Category
                {
                    Name = name
                };
                _ctx.Categories.Add(obj);
                _ctx.SaveChanges();
            }

            return obj;
        }

        public void DeleteTool(int id)
        {
            var tool = _ctx.Tools
                .FirstOrDefault(t => t.Id == id);

            if (tool is null)
                throw new ArgumentException("There is no tool object that exists with that identifier!");

            _ctx.Tools.Remove(tool);
            _ctx.SaveChanges();
        }
    }
}
