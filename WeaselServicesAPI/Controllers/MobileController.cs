using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WeaselServicesAPI.Attributes;
using WeaselServicesAPI.Models;
using WeaselServicesAPI.Services;

namespace WeaselServicesAPI.Controllers
{
    [Route("api/mobile")]
    public class MobileController : Controller
    {
        private DeviceService _service;

        public MobileController(ServicesAPIContext ctx)
        {
            _service = new DeviceService(ctx);
        }

        [HttpPost, Route("request-token")]
        public JsonResult RequestDeviceToken([FromBody] NewDeviceModel model)
        {
            try
            {
                var token = _service.CreateDevice(model.DeviceName, model.DeviceId, model.Manufacturer, Request.HttpContext.Connection.RemoteIpAddress.ToString());

                return ResponseHelper.GenerateResponse(new { Token = token }, (int)HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("ping-verify"), MobileAuthorization]
        public JsonResult VerifyPong()
        {
            return ResponseHelper.GenerateResponse(new { Message = "Pong!" }, (int)HttpStatusCode.OK);
        }
    }
}
