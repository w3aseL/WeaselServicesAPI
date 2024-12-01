using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace WeaselServicesAPI.Services
{
    public class BlogService
    {
        private readonly ServicesAPIContext _ctx;

        public BlogService(ServicesAPIContext ctx)
        {
            _ctx = ctx;
        }

        #region Blog Author

        public BlogAuthor GetBlogAuthor(string uuidStr)
        {
            Guid? uuid = null;

            if (Guid.TryParse(uuidStr, out Guid guid)) uuid = guid;

            return _ctx.BlogAuthors.FirstOrDefault(ba => ba.User.Uuid == uuid);
        }

        public BlogAuthor CreateBlogAuthor(string uuidStr, string name)
        {
            Guid? uuid = null;

            if (Guid.TryParse(uuidStr, out Guid guid)) uuid = guid;

            var user = _ctx.Users.FirstOrDefault(u => u.Uuid == uuid);

            if (user == null) throw new ArgumentException($"Could not find user with the given uuid!");

            if (_ctx.BlogAuthors.Any(a => a.UserId == user.UserId)) throw new ArgumentException("An author associated with that user already exists!");

            var blogAuthor = new BlogAuthor
            {
                Name = name,
                User = user
            };

            _ctx.BlogAuthors.Add(blogAuthor);
            _ctx.SaveChanges();

            return blogAuthor;
        }

        // TODO: Editing blog author info

        #endregion

        #region Blog Categories

        public List<BlogCategory> GetCategories()
        {
            return _ctx.BlogCategories.ToList();
        }

        public BlogCategory GetCategory(int categoryId)
        {
            var category = _ctx.BlogCategories.FirstOrDefault(c => c.CategoryId == categoryId);

            if (category == null) throw new ArgumentException("Could not find category with the identifier provided!");

            return category;
        }

        public BlogCategory CreateCategory(string categoryName, bool isHidden, string categoryColor, string categoryLogo)
        {
            if (_ctx.BlogCategories.Any(c => c.CategoryName == categoryName))
                throw new ArgumentException("A category with that name already exists!");

            var category = new BlogCategory
            {
                CategoryName = categoryName,
                IsHidden = isHidden,
                TagColor = categoryColor,
                TagIcon = categoryLogo
            };

            _ctx.BlogCategories.Add(category);
            _ctx.SaveChanges();

            return category;
        }

        #endregion

        #region Blog Posts

        public List<BlogPost> GetAllBlogPosts(bool showHidden=false)
        {
            return _ctx.BlogPosts
                .Where(bp => bp.BlogCategories.Any(c => !c.IsHidden || (showHidden && c.IsHidden)))
                .Include(bp => bp.BlogCategories)
                .Include(bp => bp.Author)
                .ToList();
        }

        public List<BlogPost> GetAllBlogPostsByAuthor(int authorId, bool showHidden=false)
        {
            return _ctx.BlogPosts
                .Include(bp => bp.BlogCategories)
                .Include(bp => bp.Author)
                .Where(bp => bp.AuthorId == authorId && bp.BlogCategories.Any(c => !c.IsHidden || (showHidden && c.IsHidden)))
                .ToList();
        }

        public BlogPost GetBlogPost(int postId)
        {
            var post = _ctx.BlogPosts.Include(p => p.BlogCategories).Include(p => p.Author).FirstOrDefault(p => p.BlogPostId == postId);

            if (post == null) new ArgumentException("Could not find blog post with the identifier provided!");

            return post;
        }

        public BlogPost CreateBlogPost(string title, string description, int authorId, List<int> categories)
        {
            if (_ctx.BlogPosts.Any(p => p.AuthorId == authorId && p.BlogTitle == title))
                throw new ArgumentException("A blog post already exists with that title!");

            var post = new BlogPost
            {
                BlogTitle = title,
                BlogDescription = description,
                AuthorId = authorId,
                DateCreated = DateTime.Now
            };

            _ctx.BlogPosts.Add(post);

            var categoryLinks = _ctx.BlogCategories
                .Where(c => categories.Contains(c.CategoryId))
                .ToList();

            foreach (var category in categoryLinks )
                post.BlogCategories.Add(category);

            _ctx.SaveChanges();

            return _ctx.BlogPosts
                .Include(p => p.BlogCategories)
                .Include(p => p.Author)
                .FirstOrDefault(p => p.BlogPostId == post.BlogPostId);
        }

        public BlogPost UpdatePostDetails(int postId, string title, string description)
        {
            var post = _ctx.BlogPosts.Include(p => p.BlogCategories).Include(p => p.Author).FirstOrDefault(p => p.BlogPostId == postId);

            if (post == null)
                throw new ArgumentException("Could not find a blog post with that identifier!");

            if (_ctx.BlogPosts.Any(p => p.AuthorId == post.AuthorId && p.BlogTitle == title && p.BlogPostId != post.BlogPostId))
                throw new ArgumentException("A blog post already exists with that title!");

            post.BlogTitle = title;
            post.BlogDescription = description;
            post.LastModified = DateTime.Now;

            _ctx.SaveChanges();

            return post;
        }

        public BlogPost UpdatePostContent(int postId, string content)
        {
            var post = _ctx.BlogPosts.Include(p => p.BlogCategories).Include(p => p.Author).FirstOrDefault(p => p.BlogPostId == postId);

            if (post == null)
                throw new ArgumentException("Could not find a blog post with that identifier!");

            post.BlogContent = content;
            post.LastModified = DateTime.Now;

            _ctx.SaveChanges();

            return post;
        }

        public void DeleteBlogPost(int postId)
        {
            var post = _ctx.BlogPosts.FirstOrDefault(p => p.BlogPostId == postId);

            if (post == null)
                throw new ArgumentException("Could not find a blog post with that identifier!");

            _ctx.BlogPosts.Remove(post);
            _ctx.SaveChanges();
        }

        #endregion
    }
}
