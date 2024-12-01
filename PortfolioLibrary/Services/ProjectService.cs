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
using System.Web;

namespace PortfolioLibrary.Services
{
    public class ProjectService
    {
        private ServicesAPIContext _ctx;
        private S3Service _s3Service;

        public ProjectService(ServicesAPIContext ctx, S3Service s3Service)
        {
            _ctx = ctx;
            _s3Service = s3Service;
        }

        public List<ProjectViewModel> GetProjects()
        {
            var projects = _ctx.Projects
                .Include(p => p.ProjectImages)
                .Include(p => p.ProjectTools)
                .ToList();

            var projectImages = projects.SelectMany(p => p.ProjectImages).Select(pi => pi.ImageId).Distinct().ToList();

            var images = _ctx.Images.Where(img => projectImages.Contains(img.Id)).ToList();
            
            var projectTools = projects.SelectMany(p => p.ProjectTools).Select(pt => pt.ToolId).Distinct().ToList();

            var tools = _ctx.Tools
                .Include(t => t.Image)
                .Include(t => t.Category)
                .Where(t => projectTools.Contains(t.Id))
                .ToList();

            return projects.Select(p => new ProjectViewModel(p, images, tools)).ToList();
        }

        public ProjectViewModel GetProject(string id)
        {
            var guid = Guid.NewGuid();

            if (Guid.TryParse(id, out Guid outGuid))
                guid = outGuid;

            var project = _ctx.Projects
                .Include(p => p.ProjectImages)
                .Include(p => p.ProjectTools)
                .FirstOrDefault(p => p.Id == guid);

            if (project is null)
                throw new ArgumentNullException("There is no project object that exists with that identifier!");

            var projectImages = project.ProjectImages.Select(pi => pi.ImageId).Distinct().ToList();

            var images = _ctx.Images.Where(img => projectImages.Contains(img.Id)).ToList();

            var projectTools = project.ProjectTools.Select(pt => pt.ToolId).Distinct().ToList();

            var tools = _ctx.Tools
                .Include(t => t.Image)
                .Include(t => t.Category)
                .Where(t => projectTools.Contains(t.Id))
                .ToList();

            return new ProjectViewModel(project, images, tools);
        }

        public ProjectViewModel CreateProject(string name, string description, string url, string repoUrl, string imageId, List<int> tools, List<string> images)
        {
            if (_ctx.Projects.Any(p => p.Name == name))
                throw new ArgumentException($"There is a project that already exists with the name \"{name}\"!");

            var project = new Project
            {
                Name = name,
                Description = description,
                Url = url,
                RepoUrl = repoUrl
            };

            _ctx.Projects.Add(project);
            _ctx.SaveChanges();

            if (tools != null && tools.Count > 0)
            {
                foreach (var toolId in tools)
                {
                    var projectTool = new ProjectTool
                    {
                        ToolId = toolId,
                        ProjectId = project.Id
                    };
                    _ctx.ProjectTools.Add(projectTool);
                }

                _ctx.SaveChanges();
            }

            // Add images
            var projectImages = new List<ProjectImage>();

            var img = GetImage(imageId);

            if (img != null)
            {
                projectImages.Add(new ProjectImage
                {
                    ProjectId = project.Id,
                    ImageId = img.Id,
                    IsLogo = true
                });
            }

            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                {
                    var addImg = GetImage(image);

                    if (addImg != null)
                        projectImages.Add(new ProjectImage
                        {
                            ProjectId = project.Id,
                            ImageId = addImg.Id,
                            IsLogo = false
                        });
                }
            }

            if (projectImages.Count > 0)
            {
                _ctx.ProjectImages.AddRange(projectImages);
                _ctx.SaveChanges();
            }

            return new ProjectViewModel(project);
        }

        public async Task<Image> UploadImage(IFormFile logoFile, bool isLogo)
        {
            using var tx = _ctx.Database.BeginTransaction();

            try
            {
                var folder = $"projects{(isLogo ? "/logo" : "")}";

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

        public ProjectViewModel UpdateProject(string id, string name, string description, string url, string repoUrl, string imageId, List<int> tools, List<string> images)
        {
            var guid = Guid.NewGuid();

            if (Guid.TryParse(id, out Guid outGuid))
                guid = outGuid;

            var project = _ctx.Projects.FirstOrDefault(e => e.Id == guid);

            if (project is null)
                throw new ArgumentNullException("There is no project object that exists with that identifier!");

            if (_ctx.Projects.Any(p => p.Name == name && p.Id != project.Id))
                throw new ArgumentException($"The project cannot be renamed to \"{name}\", as a project with that name already exists!");

            if (project.Name != name) project.Name = name;
            if (project.Description != description) project.Description = description;
            if (project.Url != url) project.Url = url;
            if (project.RepoUrl != repoUrl) project.RepoUrl = repoUrl;

            var projectTools = _ctx.ProjectTools.Where(p => p.ProjectId == project.Id).ToList();

            // Add any tools
            if (tools != null && tools.Count > 0)
            {
                foreach (var toolId in tools)
                {
                    if (!projectTools.Any(pt => pt.ToolId == toolId))
                    {
                        var projectTool = new ProjectTool
                        {
                            ToolId = toolId,
                            ProjectId = project.Id
                        };
                        _ctx.ProjectTools.Add(projectTool);
                    }
                }
            }

            // Remove tools
            var toolsToRemove = projectTools.Where(pt => !tools.Contains(pt.ToolId)).ToList();
            if (toolsToRemove.Count > 0) _ctx.ProjectTools.RemoveRange(toolsToRemove);

            var projectImages = _ctx.ProjectImages.Where(p => p.ProjectId == project.Id).ToList();

            // Check logo
            var logoImage = projectImages.FirstOrDefault(pi => pi.IsLogo);
            var logo = GetImage(imageId);

            if (logoImage != null && logo != null && logoImage.ImageId != logo.Id)
                logoImage.ImageId = logo.Id;
            if (logoImage == null && logo != null)
            {
                _ctx.ProjectImages.Add(new ProjectImage
                {
                    ProjectId = project.Id,
                    ImageId = logo.Id,
                    IsLogo = true
                });
            }

            var projectImagesNoLogo = projectImages.Where(pi => !pi.IsLogo).ToList();
            var imageGuids = images.Select(img => GetImage(img)?.Id).Where(gd => gd.HasValue).Select(gd => gd.Value).ToList();

            // add images
            if (imageGuids != null && imageGuids.Count > 0)
            {
                foreach (var image in imageGuids)
                {
                    if (!projectImagesNoLogo.Any(pi => pi.ImageId == image))
                    {
                        _ctx.ProjectImages.Add(new ProjectImage
                        {
                            ProjectId = project.Id,
                            ImageId = image,
                            IsLogo = false
                        });
                    }
                }
            }

            // remove images
            if (imageGuids != null && imageGuids.Count > 0)
            {
                var imagesToRemove = projectImagesNoLogo.Where(pi => !imageGuids.Contains(pi.ImageId)).ToList();
                if (imagesToRemove.Count > 0) _ctx.ProjectImages.RemoveRange(imagesToRemove);
            }

            _ctx.SaveChanges();

            return new ProjectViewModel(project);
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

        public void DeleteProject(string id)
        {
            var guid = Guid.NewGuid();

            if (Guid.TryParse(id, out Guid outGuid))
                guid = outGuid;

            var project = _ctx.Projects.FirstOrDefault(e => e.Id == guid);

            if (project is null)
                throw new ArgumentNullException("There is no project object that exists with that identifier!");

            var projectImages = _ctx.ProjectImages.Where(pi => pi.ProjectId == project.Id).ToList();
            _ctx.ProjectImages.RemoveRange(projectImages);

            var projectTools = _ctx.ProjectTools.Where(pt => pt.ProjectId == project.Id).ToList();
            _ctx.ProjectTools.RemoveRange(projectTools);

            _ctx.Projects.Remove(project);
            _ctx.SaveChanges();
        }
    }
}
