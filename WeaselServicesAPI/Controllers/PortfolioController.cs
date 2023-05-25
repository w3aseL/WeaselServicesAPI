using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioLibrary.Services;
using PortfolioLibrary.Models;
using System.Net;
using DataAccessLayer;
using PortfolioLibrary;

namespace WeaselServicesAPI.Controllers
{
    [Route("api/portfolio")]
    public class PortfolioController : Controller
    {
        private CategoryService _categoryService;
        private ImageService _imageService;
        private ResumeService _resumeService;

        public PortfolioController(ServicesAPIContext ctx, S3Service s3Service)
        {
            _categoryService = new CategoryService(ctx);
            _imageService = new ImageService(ctx, s3Service);
            _resumeService = new ResumeService(ctx, s3Service);
        }

        [HttpGet, Route(""), Authorize]
        public JsonResult GetPortfolio()
        {
            return ResponseHelper.GenerateResponse(new { Message = "UNIMPLEMENTED" }, (int) HttpStatusCode.OK);
        }

        [HttpGet, Route("category"), Authorize]
        public JsonResult GetCategories()
        {
            try
            {
                var categories = _categoryService.GetCategories();

                return ResponseHelper.GenerateResponse(new { Categories = categories }, (int) HttpStatusCode.OK);
            }
            catch(Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("category"), Authorize]
        public JsonResult CreateCategory(CategoryModel model)
        {
            try
            {
                var c = _categoryService.CreateCategory(model.Name);

                return ResponseHelper.GenerateResponse(new { Message = $"Created category with identifier { c.Id }!", Category = c }, (int) HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ArgumentNullException:
                    case ApplicationException:
                        return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int) HttpStatusCode.BadRequest);
                    default:
                        return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int) HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpDelete, Route("category"), Authorize]
        public JsonResult DeleteCategory(string name)
        {
            try
            {
                _categoryService.RemoveCategory(name);

                return ResponseHelper.GenerateResponse(new { Message = $"Removed category with name { name }!"}, (int) HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ApplicationException:
                        return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
                    default:
                        return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpGet, Route("image"), Authorize]
        public JsonResult GetImages()
        {
            try
            {
                var images = _imageService.GetImages();

                return ResponseHelper.GenerateResponse(new { Images = images }, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("image/{id:guid}"), Authorize]
        public JsonResult GetImage(string id)
        {
            try
            {
                var image = _imageService.GetImage(id);

                return ResponseHelper.GenerateResponse(new { Id = image.Id, FileName = image.FileName, Url = image.Url }, (int)HttpStatusCode.OK);
            }
            catch(Exception e)
            {
                switch (e)
                {
                    case ArgumentNullException:
                        return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
                    default:
                        return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpDelete, Route("image"), Authorize]
        public async Task<JsonResult> DeleteImage(string id)
        {
            try
            {
                await _imageService.DeleteImage(id);

                return ResponseHelper.GenerateResponse(new { Message = $"Removed image with identifier { id }!" }, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ArgumentNullException:
                        return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
                    default:
                        return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpGet, Route("resume"), Authorize]
        public JsonResult GetResumes()
        {
            try
            {
                var res = _resumeService.GetResumes();

                return ResponseHelper.GenerateResponse(new { Resumes = res }, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("resume"), Authorize]
        public async Task<JsonResult> CreateResume(ResumeModel model)
        {
            try
            {
                var res = await _resumeService.UploadResume(model.File, model.CreationDate);

                return ResponseHelper.GenerateResponse(new { Message = $"Successfully uploaded resume!", Id = res.Id, Url = res.Url }, (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete, Route("resume/{id:guid}"), Authorize]
        public async Task<JsonResult> DeleteResume(string id)
        {
            try
            {
                await _resumeService.DeleteResume(id);

                return ResponseHelper.GenerateResponse(new { Message = $"Removed resume with identifier { id }!" }, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ArgumentNullException:
                        return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
                    default:
                        return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}
