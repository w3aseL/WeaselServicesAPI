using EmailService;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System;
using WeaselServicesAPI.Services;
using EmailService.Models;
using WeaselServicesAPI.Models.Email;
using WeaselServicesAPI.Models;

namespace WeaselServicesAPI.Controllers
{
    [Route("api/test")]
    public class TestController : Controller
    {
        private readonly IEmailSender _emailSender;
        
        public TestController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost, Route("sample-email")]
        public JsonResult SendTestEmail([FromBody] TestSampleEmailModel model)
        {
            try
            {
                var emailAddr = model.Email;
                var message = new ModeledMessage<SampleEmail>(new List<string> { emailAddr }, "This is a sample email!", new SampleEmail
                {
                    FirstValue = "Person",
                    SecondValue = emailAddr
                });

                _emailSender.SendEmailWithModel(message);

                return ResponseHelper.GenerateResponse(
                    new { Message = $"Sent sample email to \"{ emailAddr }\"!" },
                    (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
