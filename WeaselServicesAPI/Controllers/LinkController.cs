using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioLibrary.Services;
using System.Net;
using WeaselServicesAPI.Services;

namespace WeaselServicesAPI.Controllers
{
    [Route("api/link")]
    public class LinkController : Controller
    {
        private LinkTreeService _service;

        public LinkController(ServicesAPIContext ctx)
        {
            _service = new LinkTreeService(ctx);
        }

        [HttpGet, AllowAnonymous]
        public JsonResult GetAllLinks()
        {
            try
            {
                return ResponseHelper.GenerateResponse(_service.GetAllLinks(), (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Authorize]
        public JsonResult CreateLink([FromBody] LinkModel model)
        {
            try
            {
                return ResponseHelper.GenerateResponse(_service.CreateLink(model), (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch, HttpPut, Authorize]
        public JsonResult UpdateLink([FromBody] LinkModel model)
        {
            try
            {
                return ResponseHelper.GenerateResponse(_service.EditLink(model), (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete, Route("{linkId:int}"), Authorize]
        public JsonResult DeleteLink(int linkId)
        {
            try
            {
                _service.DeleteLink(linkId);

                return ResponseHelper.GenerateResponse(new {}, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
