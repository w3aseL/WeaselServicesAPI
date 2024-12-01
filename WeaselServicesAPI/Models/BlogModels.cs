using DataAccessLayer.Models;
using Newtonsoft.Json;

namespace WeaselServicesAPI
{
    public class NewBlogAuthor
    {
        public string Name { get; set; }
    }

    public class NewBlogCategory
    {
        public string CategoryName { get; set; }
        public bool IsHidden { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? CategoryColor { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? CategoryLogo { get; set; }
    }

    public class NewBlogPost
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<int> Categories { get; set; }
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
        public string CategoryColor { get; set; }
        public string CategoryLogo { get; set; }
        public bool IsHidden { get; set; }

        public BlogCategoryModel() { }

        public BlogCategoryModel(BlogCategory category)
        {
            CategoryId = category.CategoryId;
            CategoryName = category.CategoryName;
            CategoryColor = category.TagColor;
            CategoryLogo = category.TagIcon;
            IsHidden = category.IsHidden;
        }
    }

    public class BlogPostModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public BlogAuthorModel Author { get; set; }
        public List<BlogCategoryModel> Categories { get; set; }

        public BlogPostModel(BlogPost post)
        {
            PostId = post.BlogPostId;
            Title = post.BlogTitle;
            Description = post.BlogDescription;
            Content = post.BlogContent;
            Author = new BlogAuthorModel(post.Author);
            Categories = post.BlogCategories.Select(c => new BlogCategoryModel(c)).ToList();
        }
    }
}
