using DataAccessLayer.Models;

namespace WeaselServicesAPI
{
    public class NewBlogAuthor
    {
        public string Name { get; set; }
    }

    public class NewBlogCategory
    {
        public string Name { get; set; }
        public bool IsHidden { get; set; }
    }

    public class NewBlogPost
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateBlogPostDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class UpdateBlogPostContent
    {
        public string Content { get; set; }
    }

    public class BlogAuthorModel
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }

        public BlogAuthorModel(BlogAuthor author)
        {
            AuthorId = author.Id;
            Name = author.Name;
        }
    }

    public class BlogCategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public BlogCategoryModel() { }

        public BlogCategoryModel(BlogCategory category)
        {
            CategoryId = category.CategoryId;
            CategoryName = category.CategoryName;
        }
    }

    public class BlogPostModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public BlogAuthorModel Author { get; set; }
        public BlogCategoryModel Category { get; set; }

        public BlogPostModel(BlogPost post)
        {
            PostId = post.BlogId;
            Title = post.BlogTitle;
            Description = post.BlogDescription;
            Content = post.BlogContent;
            Author = new BlogAuthorModel(post.Author);
            Category= new BlogCategoryModel(post.Category);
        }
    }
}
