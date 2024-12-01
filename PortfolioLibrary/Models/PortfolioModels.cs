using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioLibrary.Models
{
    public class CategoryModel
    {
        public string Name { get; set; }
    }

    public class DeleteModel
    {
        [Required]
        public string Id { get; set; }
    }

    public class ResumeModel
    {
        [Required]
        public DateTime CreationDate { get; set; }
    }

    public class EducationModel
    {
        [Required]
        public string SchoolName { get; set; }
        [Required]
        public string SchoolType { get; set; }
        [Required]
        public string RewardType { get; set; }
        [Required]
        public DateTime GraduationDate { get; set; }
        [Required]
        public decimal GPA { get; set; }
        [Required]
        public string SchoolURL { get; set; }   
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Major { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ImageId { get; set; }
    }

    public class EducationViewModel
    {
        public string Id { get; set; }
        public string SchoolName { get; set; }
        public string SchoolType { get; set; }
        public string RewardType { get; set; }
        public DateTime GraduationDate { get; set; }
        public decimal GPA { get; set; }
        public string SchoolURL { get; set; }
        public string? Major { get; set; }
        public string LogoURL { get; set; }

        public EducationViewModel() { }

        public EducationViewModel(Education e)
        {
            Id = e.Id.ToString();
            SchoolName = e.SchoolName;
            SchoolType = e.SchoolType;
            RewardType = e.RewardType;
            GraduationDate = e.GraduationDate;
            GPA = e.Gpa;
            SchoolURL = e.SchoolUrl;
            Major = e.Major;

            if (e.Image != null) LogoURL = e.Image.Url;
        }
    }

    public class PositionModel
    {
        [Required]
        public string JobTitle { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string CompanyURL { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        public DateTime? EndDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ImageId { get; set; }
    }

    public class PositionViewModel
    {
        public string Id { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string CompanyURL { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ImageId { get; set; }
        public string LogoURL { get; set; }

        public PositionViewModel() { }

        public PositionViewModel(Position p)
        {
            Id = p.Id.ToString();
            JobTitle = p.JobTitle;
            CompanyName = p.CompanyName;
            CompanyURL = p.CompanyUrl;
            Description = p.Description;
            StartDate = p.StartDate;
            EndDate = p.EndDate;

            if (p.Image != null)
            {
                ImageId = p.Image.Id.ToString();
                LogoURL = p.Image.Url;
            }
        }
    }

    public class ToolModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string URL { get; set; }
        [Required]
        public string Category { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ImageId { get; set; }

    }

    public class ToolViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string Category { get; set; }
        public string ImageId { get; set; }
        public string LogoURL { get; set; }

        public ToolViewModel() { }

        public ToolViewModel(Tool t)
        {
            Id = t.Id;
            Name = t.Name;
            Description = t.Description;
            URL = t.Url;
            if (t.Category != null) Category = t.Category.Name;
            if (t.Image != null)
            {
                ImageId = t.Image.Id.ToString();
                LogoURL = t.Image.Url;
            }
        }
    }

    public class ProjectModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string URL { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RepoURL { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ImageId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> Tools { get; set; } = new List<int>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Images { get; set; } = new List<string>();
    }

    public class ProjectViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string RepoURL { get; set; }
        public string LogoURL { get; set; }
        public List<string> Images { get; set; }
        public List<ToolViewModel> Tools { get; set; }

        public ProjectViewModel() { }

        public ProjectViewModel(Project project)
        {
            Id = project.Id.ToString();
            Name = project.Name;
            Description = project.Description;
            URL = project.Url;
            RepoURL = project.RepoUrl;
        }

        public ProjectViewModel(Project project, List<Image> images, List<Tool> tools)
        {
            Id = project.Id.ToString();
            Name = project.Name;
            Description = project.Description;
            URL = project.Url;
            RepoURL = project.RepoUrl;

            var projectImageIds = project.ProjectImages.Where(pi => !pi.IsLogo).Select(pi => pi.ImageId).ToList();
            var imageList = images.Where(img => projectImageIds.Contains(img.Id)).ToList();
            var projectLogoId = project.ProjectImages.FirstOrDefault(pi => pi.IsLogo)?.ImageId;
            var logo = images.FirstOrDefault(img => img.Id == projectLogoId);

            if (logo != null) LogoURL = logo.Url;

            Images = imageList.Select(img => img.Url).ToList();

            var projectToolIds = project.ProjectTools.Select(pi => pi.ToolId).ToList();
            var toolList = tools.Where(t => projectToolIds.Contains(t.Id)).ToList();

            Tools = toolList.Select(t => new ToolViewModel(t)).ToList();
        }
    }

    public class LogoModel
    {
        [Required]
        public IFormFile File { get; set; }
    }

    public class ProjectImageModel
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public bool IsLogo { get; set; }
    }
}
