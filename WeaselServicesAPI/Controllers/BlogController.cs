using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Security.Claims;
using WeaselServicesAPI.Exceptions;
using WeaselServicesAPI.Services;

namespace WeaselServicesAPI.Controllers
{
    [Route("api/blog")]
    public class BlogController : Controller
    {
        private BlogService _blogService;

        public BlogController(ServicesAPIContext ctx)
        {
            _blogService = new BlogService(ctx);
        }


        [HttpGet, Route("")]
        public JsonResult GetAllPosts()
        {
            return ResponseHelper.GenerateResponse(
                _blogService.GetAllBlogPosts().Select(p => new BlogPostModel(p)).ToList(),
                (int)HttpStatusCode.OK);
        }

        #region Author

        [HttpGet, Route("author"), Authorize]
        public JsonResult GetUserAuthor()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var uuid = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var author = _blogService.GetBlogAuthor(uuid);

                return ResponseHelper.GenerateResponse(
                    author != null ? new BlogAuthorModel(author) : new { Message = "Could not find author associated to that user!" },
                    author != null ? (int)HttpStatusCode.OK : (int) HttpStatusCode.BadRequest);
            }
            catch (ArgumentException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("author"), Authorize]
        public JsonResult CreateBlogAuthor([FromBody] NewBlogAuthor model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var uuid = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                return ResponseHelper.GenerateResponse(new BlogAuthorModel(_blogService.CreateBlogAuthor(uuid, model.Name)), (int)HttpStatusCode.OK);
            }
            catch (ArgumentException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region Categories

        [HttpGet, Route("category"), Authorize]
        public JsonResult GetAllCategories()
        {
            return ResponseHelper.GenerateResponse(
                _blogService.GetCategories().Select(c => new BlogCategoryModel(c)).ToList(),
                (int)HttpStatusCode.OK);
        }

        [HttpGet, Route("category/{id:int}"), Authorize]
        public JsonResult GetCategory(int id)
        {
            try
            {
                return ResponseHelper.GenerateResponse(
                    new BlogCategoryModel(_blogService.GetCategory(id)),
                    (int)HttpStatusCode.OK);
            }
            catch (ArgumentException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("category"), Authorize]
        public JsonResult CreateCategory([FromBody] NewBlogCategory model)
        {
            try
            {
                return ResponseHelper.GenerateResponse(
                    new BlogCategoryModel(_blogService.CreateCategory(model.CategoryName, model.IsHidden, model.CategoryColor ?? "#fff", model.CategoryLogo)),
                    (int)HttpStatusCode.OK);
            }
            catch (ArgumentException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region Posts

        [HttpGet, Route("post"), Authorize]
        public JsonResult GetAllUserPosts()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var uuid = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var author = _blogService.GetBlogAuthor(uuid);

                return ResponseHelper.GenerateResponse(
                    author != null ? _blogService.GetAllBlogPostsByAuthor(author.Id, true).Select(p => new BlogPostModel(p)).ToList() : new { Message = "Could not get posts associated to that user!" },
                    author != null ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NotFound);
            }
            catch (ArgumentException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("post/{id:int}"), Authorize]
        public JsonResult GetUserPost(int id)
        {
            try
            {
                return ResponseHelper.GenerateResponse(
                    new BlogPostModel(_blogService.GetBlogPost(id)),
                    (int)HttpStatusCode.OK);
            }
            catch (ArgumentException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("post"), Authorize]
        public JsonResult CreateBlogPost([FromBody] NewBlogPost model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var uuid = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var author = _blogService.GetBlogAuthor(uuid);

                return ResponseHelper.GenerateResponse(
                    author != null ? new BlogPostModel(_blogService.CreateBlogPost(model.Title, model.Description, author.Id, model.Categories)) : new { Message = "Could not create a post associated to that user!" },
                    author != null ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NotFound);
            }
            catch (ArgumentException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch, HttpPut, Route("post/{id:int}"), Authorize]
        public JsonResult UpdateBlogPostDetails(int id, [FromBody] UpdateBlogPostDetails model)
        {
            try
            {
                return ResponseHelper.GenerateResponse(
                    new BlogPostModel(_blogService.UpdatePostDetails(id, model.Title, model.Description)),
                    (int)HttpStatusCode.OK);
            }
            catch (ArgumentException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("post/{id:int}"), Authorize]
        public JsonResult UpdateBlogPostContent(int id, [FromBody] UpdateBlogPostContent model)
        {
            try
            {
                return ResponseHelper.GenerateResponse(
                    new BlogPostModel(_blogService.UpdatePostContent(id, model.Content)),
                    (int)HttpStatusCode.OK);
            }
            catch (ArgumentException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete, Route("post/{id:int}"), Authorize]
        public JsonResult DeleteBlogPost(int id)
        {
            try
            {
                _blogService.DeleteBlogPost(id);

                return ResponseHelper.GenerateResponse(null, (int)HttpStatusCode.NoContent);
            }
            catch (ArgumentException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        #endregion
    }
}
