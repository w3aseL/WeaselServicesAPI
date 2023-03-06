using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPILibrary;
using System.Net;
using System.Security.Claims;
using WeaselServicesAPI.Exceptions;
using WeaselServicesAPI.Services;

namespace WeaselServicesAPI.Controllers
{
    [Route("api/spotify")]
    public class SpotifyController : Controller
    {
        private readonly SpotifyService _service;
        private readonly ServicesAPIContext _ctx;

        public SpotifyController(ISpotifyAPI api, ServicesAPIContext ctx)
        {
            _service = new SpotifyService(api);
            _ctx = ctx;
        }

        [HttpGet, Route("test"), Authorize]
        public async Task<JsonResult> Test()
        {
            try
            {
                var obj = await _service.GetTestTrack();

                return ResponseHelper.GenerateResponse(obj, (int) HttpStatusCode.OK);
            }
            catch (UserExistsException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("login"), Authorize]
        public async Task<JsonResult> StartLoginSequence()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var uuid = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var userId = _ctx.Users.Where(u => u.Uuid.ToString() == uuid).FirstOrDefault()?.UserId ?? -1;

                var uri = _service.GetNewAccountUri(userId, $"{Request.Scheme}://{Request.Host}/api/spotify/auth");

                return ResponseHelper.GenerateResponse(new { url = uri }, (int)HttpStatusCode.Created);
            }
            catch (UserExistsException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("auth"), AllowAnonymous]
        public async Task<IActionResult> AccountCallback()
        {
            try
            {
                var code = Request.Query["code"];
                var state = Request.Query["state"];

                await _service.ConfirmAccountWithCode(code, state, $"{Request.Scheme}://{Request.Host}/api/spotify/auth");

                return RedirectToAction("SuccessfulAccountLogin", new { message = "Successfully logged into spotify account! No redirect after login was provided." });
            }
            catch (UserExistsException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("login-success"), AllowAnonymous]
        public JsonResult SuccessfulAccountLogin(string? message)
        {
            if(message is null)
                return ResponseHelper.GenerateResponse(null, (int)HttpStatusCode.Unauthorized);

            return ResponseHelper.GenerateResponse(new { Message = message }, (int)HttpStatusCode.OK);
        }

        [HttpGet, Route("current-song"), Authorize]
        public JsonResult CurrentSong()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var uuid = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var userId = _ctx.Users.Where(u => u.Uuid.ToString() == uuid).FirstOrDefault()?.UserId ?? -1;

                var obj = _service.GetCurrentlyPlayedSong(userId);

                return ResponseHelper.GenerateResponse(obj, (int)HttpStatusCode.OK);
            }
            catch (UserExistsException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("tracking-all"), Authorize]
        public JsonResult CurrentPlayerStatusAll()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var uuid = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var userId = _ctx.Users.Where(u => u.Uuid.ToString() == uuid).FirstOrDefault()?.UserId ?? -1;

                var obj = _service.GetPlayerStatus(userId);

                return ResponseHelper.GenerateResponse(obj, (int)HttpStatusCode.OK);
            }
            catch (UserExistsException e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
