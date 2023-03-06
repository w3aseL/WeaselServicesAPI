using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
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
}
