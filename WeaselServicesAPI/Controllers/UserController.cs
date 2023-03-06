using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WeaselServicesAPI.Exceptions;
using WeaselServicesAPI.Helpers;
using WeaselServicesAPI.Helpers.Interfaces;
using WeaselServicesAPI.Services;

namespace WeaselServicesAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserService _service;

        public UserController(ServicesAPIContext ctx, IEmailSender sender, ITokenGenerator tokenGen)
        {
            _service = new UserService(ctx, sender, tokenGen);
        }

        [HttpPost, Route("register")]
        public JsonResult Register(RegisterModel model)
        {
            try
            {
                var user = _service.RegisterNewUser(model.Username, model.Email);

                return ResponseHelper.GenerateResponse(new { Message = $"A new user has been created with the username \"{ model.Username }.\" An email has been sent with registration instructions." }, (int) HttpStatusCode.Created);
            }
            catch (UserExistsException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int) HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("confirm-registration")]
        public JsonResult ConfirmRegistration(ConfirmRegistrationModel model)
        {
            try
            {
                if (model.Password != model.ConfirmPassword)
                    return ResponseHelper.GenerateResponse(new { Message = "The passwords provided do not match!" }, (int) HttpStatusCode.BadRequest);

                var success = _service.ConfirmUserRegistration(model.RequestCode, model.Password);

                if (success)
                    return ResponseHelper.GenerateResponse(new { Message = $"Successfully updated password for user." }, (int) HttpStatusCode.OK);
                else
                    return ResponseHelper.GenerateResponse(new { Message = "Somehow failed to successfully update password." }, (int)HttpStatusCode.BadRequest);
            }
            catch (UserNotFoundException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("login")]
        public JsonResult Login(LoginModel model)
        {
            try
            {
                var res = _service.LoginUser(model.Username, model.Password);

                return ResponseHelper.GenerateResponse(res, (int) HttpStatusCode.OK);
            }
            catch (UserNotFoundException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("refresh")]
        public JsonResult RefreshToken(RefreshModel model)
        {
            try
            {
                var accessToken = _service.RefreshAccessToken(model.RefreshToken);

                return ResponseHelper.GenerateResponse(new { AccessToken = accessToken }, (int)HttpStatusCode.OK);
            }
            catch (UserNotFoundException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int) HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int) HttpStatusCode.InternalServerError);
            }
        }

        // TODO: Change password route
    }
}
