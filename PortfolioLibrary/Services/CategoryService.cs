using DataAccessLayer;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioLibrary.Services
{
    public class CategoryService
    {
        private ServicesAPIContext _ctx;
        
        public CategoryService(ServicesAPIContext ctx)
        {
            _ctx = ctx;
        }

        public Category CreateCategory(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("No name provided to create a category!");

            if (_ctx.Categories.FirstOrDefault(c => c.Name == name) is null)
                throw new ApplicationException("A category with that name already exists!");

            var category = new Category()
            {
                Name = name
            };

            _ctx.Categories.Add(category);
            _ctx.SaveChanges();

            return category;
        }

        public List<Category> GetCategories()
        {
            return _ctx.Categories.ToList();
        }

        public void RemoveCategory(string name)
        {
            var category = _ctx.Categories.FirstOrDefault(c => c.Name == name);

            if (category is null)
                throw new ApplicationException("A category with that name does not exists!");

            _ctx.Remove(category);
            _ctx.SaveChanges();
        }
    }
}
