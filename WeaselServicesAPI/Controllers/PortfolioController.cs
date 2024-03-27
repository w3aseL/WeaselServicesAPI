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
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private CategoryService _categoryService;
        private ImageService _imageService;
        private ResumeService _resumeService;
        private EducationService _educationService;
        private PositionService _positionService;

        public PortfolioController(ServicesAPIContext ctx, S3Service s3Service)
        {
            _categoryService = new CategoryService(ctx);
            _imageService = new ImageService(ctx, s3Service);
            _resumeService = new ResumeService(ctx, s3Service);
            _educationService = new EducationService(ctx, s3Service);
            _positionService = new PositionService(ctx, s3Service);
        }

        [HttpGet, Route(""), AllowAnonymous]
        public JsonResult GetPortfolio()
        {
            return ResponseHelper.GenerateResponse(new { Message = "UNIMPLEMENTED" }, (int) HttpStatusCode.OK);
        }

        #region Categories

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
        public JsonResult CreateCategory([FromBody] CategoryModel model)
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

        [HttpDelete, Route("category/{id:int}"), Authorize]
        public JsonResult DeleteCategory(int id)
        {
            try
            {
                _categoryService.RemoveCategory(id);

                return ResponseHelper.GenerateResponse(new { Message = $"Removed category with identifier {id}!"}, (int) HttpStatusCode.OK);
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

        #endregion

        #region Images

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

        #endregion

        #region Resumes

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

        [HttpDelete, Route("resume/{id}"), Authorize]
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

        #endregion

        #region Education

        [HttpGet, Route("education"), Authorize]
        public JsonResult GetEducations()
        {
            try
            {
                var educations = _educationService.GetEducations();

                return ResponseHelper.GenerateResponse(educations, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("education/{id}"), Authorize]
        public JsonResult GetEducations(string id)
        {
            try
            {
                var education = _educationService.GetEducation(id);

                return ResponseHelper.GenerateResponse(education, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("education"), Authorize]
        public JsonResult CreateEduction(EducationModel model)
        {
            try
            {
                var education = _educationService.CreateEducation(model.SchoolName, model.SchoolType, model.RewardType, model.GraduationDate, model.GPA, model.SchoolURL, model.Major);

                return ResponseHelper.GenerateResponse(education, (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        // TODO: PATCH/PUT for updates

        [HttpPost, Route("education/{id}/logo"), Authorize, Consumes("multipart/form-data")]
        public async Task<JsonResult> SetSchoolLogo(string id, [FromForm] LogoModel model)
        {
            try
            {
                await _educationService.SetLogo(id, model.File);

                return ResponseHelper.GenerateResponse(new {}, (int)HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete, Route("education/{id}"), Authorize]
        public JsonResult DeleteEducation(string id)
        {
            try
            {
                _educationService.DeleteEducation(id);

                return ResponseHelper.GenerateResponse(new { Message = $"Removed education with identifier {id}!" }, (int)HttpStatusCode.OK);
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

        #endregion

        #region Positions

        [HttpGet, Route("position"), Authorize]
        public JsonResult GetPositions()
        {
            try
            {
                var positions = _positionService.GetPositions();

                return ResponseHelper.GenerateResponse(positions, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("position/{id}"), Authorize]
        public JsonResult GetPosition(string id)
        {
            try
            {
                var pos = _positionService.GetPosition(id);

                return ResponseHelper.GenerateResponse(pos, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("position"), Authorize]
        public JsonResult CreatePosition(PositionModel model)
        {
            try
            {
                // var education = _educationService.CreateEducation(model.SchoolName, model.SchoolType, model.RewardType, model.GraduationDate, model.GPA, model.SchoolURL, model.Major);
                var pos = _positionService.CreatePosition(model.JobTitle, model.CompanyName, model.CompanyURL, model.Description, model.StartDate, model.EndDate);

                return ResponseHelper.GenerateResponse(pos, (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        // TODO: PATCH/PUT for updates

        [HttpPost, Route("position/{id}/logo"), Authorize, Consumes("multipart/form-data")]
        public async Task<JsonResult> SetCompanyLogo(string id, [FromForm] LogoModel model)
        {
            try
            {
                await _positionService.SetLogo(id, model.File);

                return ResponseHelper.GenerateResponse(new { }, (int)HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete, Route("position/{id}"), Authorize]
        public JsonResult DeletePosition(string id)
        {
            try
            {
                _positionService.DeletePosition(id);

                return ResponseHelper.GenerateResponse(new { Message = $"Removed position with identifier {id}!" }, (int)HttpStatusCode.OK);
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

        #endregion

        #region Tools

        // TODO

        #endregion

        #region Projects 

        // TODO

        #endregion

    }
}
