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

    public class ResumeModel
    {
        public IFormFile File { get; set; }
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
    }

    public class LogoModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
