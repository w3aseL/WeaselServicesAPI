using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioLibrary.Services;
using PortfolioLibrary.Models;
using System.Net;
using DataAccessLayer;
using PortfolioLibrary;
using DataAccessLayer.Models;

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
        private ToolService _toolService;
        private ProjectService _projectService;

        public PortfolioController(ServicesAPIContext ctx, S3Service s3Service)
        {
            _categoryService = new CategoryService(ctx);
            _imageService = new ImageService(ctx, s3Service);
            _resumeService = new ResumeService(ctx, s3Service);
            _educationService = new EducationService(ctx, s3Service);
            _positionService = new PositionService(ctx, s3Service);
            _toolService = new ToolService(ctx, s3Service);
            _projectService = new ProjectService(ctx, s3Service);
        }

        [HttpGet, Route(""), AllowAnonymous]
        public JsonResult GetPortfolio()
        {
            var education = _educationService.GetEducationsNoModel()
                .Select(e => new
                {
                    e.Id,
                    e.SchoolName,
                    e.SchoolType,
                    SchoolURL = e.SchoolUrl,
                    e.RewardType,
                    e.Major,
                    e.GraduationDate,
                    e.Gpa,
                    SchoolLogo = e.Image?.Url ?? ""
                }).ToList();

            var resumes = _resumeService.GetResumes()
                .Select(r => new
                {
                    r.Id,
                    r.Url,
                    r.CreationDate
                }).ToList();

            var positions = _positionService.GetPositions()
                .Select(s => new
                {
                    s.Id,
                    s.JobTitle,
                    s.CompanyName,
                    CompanyURL = s.CompanyUrl,
                    s.Description,
                    s.StartDate,
                    s.EndDate,
                    LogoURL = s.Image?.Url ?? ""
                })
                .OrderBy(s => s.StartDate)
                .ToList();
            var tools = _toolService.GetTools()
                .Select(t => new ToolViewModel(t))
                .GroupBy(t => t.Category)
                .ToDictionary(x => x.Key, x => x.ToList());
            var projects = _projectService
                .GetProjects();

            return ResponseHelper.GenerateResponse(new {
                Tools = tools,
                Resumes = resumes,
                Education = education,
                Projects = projects,
                Positions = positions
            }, (int) HttpStatusCode.OK);
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

                return ResponseHelper.GenerateResponse(images, (int)HttpStatusCode.OK);
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

        [HttpDelete, Route("image/{id:guid}"), Authorize]
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

                return ResponseHelper.GenerateResponse(res, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("resume"), Authorize, Consumes("multipart/form-data")]
        public async Task<JsonResult> CreateResume([FromForm] LogoModel model)
        {
            try
            {
                var res = await _resumeService.UploadResume(model.File);

                return ResponseHelper.GenerateResponse(new { Message = $"Successfully uploaded resume!", Id = res.Id, Url = res.Url }, (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch, Route("resume/{id:guid}"), Authorize]
        public async Task<JsonResult> UpdateResume(string id, [FromBody] ResumeModel model)
        {
            try
            {
                var res = _resumeService.UpdateResumeDate(id, model.CreationDate);

                return ResponseHelper.GenerateResponse(res, (int)HttpStatusCode.Created);
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
                var education = _educationService.CreateEducation(model.SchoolName, model.SchoolType, model.RewardType, model.GraduationDate, model.GPA, model.SchoolURL, model.Major, model.ImageId);

                return ResponseHelper.GenerateResponse(education, (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        // TODO: PATCH/PUT for updates

        [HttpPost, Route("education/logo"), Authorize, Consumes("multipart/form-data")]
        public async Task<JsonResult> UploadSchoolLogo([FromForm] LogoModel model)
        {
            try
            {
                var img = await _educationService.UploadLogo(model.File);

                return ResponseHelper.GenerateResponse(img, (int)HttpStatusCode.Created);
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
                var positions = _positionService.GetPositionsWithModel();

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
        public JsonResult CreatePosition([FromBody] PositionModel model)
        {
            try
            {
                // var education = _educationService.CreateEducation(model.SchoolName, model.SchoolType, model.RewardType, model.GraduationDate, model.GPA, model.SchoolURL, model.Major);
                var pos = _positionService.CreatePosition(model.JobTitle, model.CompanyName, model.CompanyURL, model.Description, model.StartDate, model.EndDate, model.ImageId);

                return ResponseHelper.GenerateResponse(pos, (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch, HttpPut, Route("position/{id:guid}"), Authorize]
        public JsonResult UpdatePosition(string id, [FromBody] PositionModel model)
        {
            try
            {
                // var education = _educationService.CreateEducation(model.SchoolName, model.SchoolType, model.RewardType, model.GraduationDate, model.GPA, model.SchoolURL, model.Major);
                var pos = _positionService.UpdatePosition(id, model.JobTitle, model.CompanyName, model.CompanyURL, model.Description, model.StartDate, model.EndDate, model.ImageId);

                return ResponseHelper.GenerateResponse(pos, (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("position/logo"), Authorize, Consumes("multipart/form-data")]
        public async Task<JsonResult> SetCompanyLogo([FromForm] LogoModel model)
        {
            try
            {
                var img = await _positionService.UploadLogo(model.File);

                return ResponseHelper.GenerateResponse(img, (int)HttpStatusCode.Created);
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

        [HttpGet, Route("tool"), Authorize]
        public JsonResult GetTools()
        {
            try
            {
                var tools = _toolService.GetTools().Select(t => new ToolViewModel(t)).ToList();

                return ResponseHelper.GenerateResponse(tools, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("tool/{id:int}"), Authorize]
        public JsonResult GetTool(int id)
        {
            try
            {
                var tool = _toolService.GetTool(id);

                return ResponseHelper.GenerateResponse(new ToolViewModel(tool), (int)HttpStatusCode.OK);
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

        [HttpPost, Route("tool"), Authorize]
        public JsonResult CreateTool(ToolModel model)
        {
            try
            {
                var tool = _toolService.CreateTool(model.Name, model.URL, model.Description, model.Category, model.ImageId);

                return ResponseHelper.GenerateResponse(new ToolViewModel(tool), (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut, HttpPatch, Route("tool/{id:int}"), Authorize]
        public JsonResult UpdateTool(int id, [FromBody] ToolModel model)
        {
            try
            {
                var tool = _toolService.UpdateTool(id, model.Name, model.URL, model.Description, model.Category, model.ImageId);

                return ResponseHelper.GenerateResponse(new ToolViewModel(tool), (int)HttpStatusCode.OK);
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

        [HttpPost, Route("tool/logo"), Authorize, Consumes("multipart/form-data")]
        public async Task<JsonResult> SetToolLogo([FromForm] LogoModel model)
        {
            try
            {
                var image = await _toolService.UploadLogo(model.File);

                return ResponseHelper.GenerateResponse(image, (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete, Route("tool/{id:int}"), Authorize]
        public JsonResult DeleteTool(int id)
        {
            try
            {
                _toolService.DeleteTool(id);

                return ResponseHelper.GenerateResponse(new { Message = $"Removed tool with identifier {id}!" }, (int)HttpStatusCode.OK);
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

        #region Projects 

        [HttpGet, Route("project"), Authorize]
        public JsonResult GetProjects()
        {
            try
            {
                var projects = _projectService.GetProjects();

                return ResponseHelper.GenerateResponse(projects, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("project/{id}"), Authorize]
        public JsonResult GetProject(string id)
        {
            try
            {
                var project = _projectService.GetProject(id);

                return ResponseHelper.GenerateResponse(project, (int)HttpStatusCode.OK);
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

        [HttpPost, Route("project"), Authorize]
        public JsonResult CreateProject(ProjectModel model)
        {
            try
            {
                var project = _projectService.CreateProject(model.Name, model.Description, model.URL, model.RepoURL, model.ImageId, model.Tools, model.Images);

                return ResponseHelper.GenerateResponse(project, (int)HttpStatusCode.Created);
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

        [HttpPatch, HttpPut, Route("project/{id}"), Authorize]
        public JsonResult UpdateProject(string id, [FromBody] ProjectModel model)
        {
            try
            {
                var project = _projectService.UpdateProject(id, model.Name, model.Description, model.URL, model.RepoURL, model.ImageId, model.Tools, model.Images);

                return ResponseHelper.GenerateResponse(project, (int)HttpStatusCode.Created);
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

        [HttpPost, Route("project/upload-image"), Authorize, Consumes("multipart/form-data")]
        public async Task<JsonResult> UploadProjectImage([FromForm] LogoModel model)
        {
            try
            {
                var img = await _projectService.UploadImage(model.File, false);

                return ResponseHelper.GenerateResponse(img, (int)HttpStatusCode.Created);
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

        [HttpPost, Route("project/upload-logo"), Authorize, Consumes("multipart/form-data")]
        public async Task<JsonResult> SetProjectLogo([FromForm] LogoModel model)
        {
            try
            {
                var img = await _projectService.UploadImage(model.File, true);

                return ResponseHelper.GenerateResponse(img, (int)HttpStatusCode.Created);
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

        [HttpDelete, Route("project/{id}"), Authorize]
        public JsonResult DeleteProject(string id)
        {
            try
            {
                _projectService.DeleteProject(id);

                return ResponseHelper.GenerateResponse(new { Message = $"Removed project with identifier {id}!" }, (int)HttpStatusCode.OK);
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

    }
}
