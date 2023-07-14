using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyAPILibrary;
using System.Data.SqlTypes;
using System.Net;
using System.Security.Claims;
using WeaselServicesAPI.Exceptions;
using WeaselServicesAPI.Services;

namespace WeaselServicesAPI.Controllers
{
    [Route("api/spotify")]
    public class SpotifyController : Controller
    {
        private readonly ISpotifyAPI _spotify;
        private readonly ServicesAPIContext _ctx;

        public SpotifyController(ISpotifyAPI api, ServicesAPIContext ctx)
        {
            _spotify = api;
            _ctx = ctx;
        }

        [HttpGet, Route("test"), Authorize]
        public async Task<JsonResult> Test()
        {
            try
            {
                var obj = await _spotify.GetTrack();

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

                var uri = _spotify.GetAccountRequestUrl(userId, $"{Request.Scheme}://{Request.Host}/api/spotify/auth");

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

                await _spotify.CompleteAccountRequest(code, state, $"{Request.Scheme}://{Request.Host}/api/spotify/auth");

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

                var obj = _spotify.GetCurrentlyListenedToSong(userId);

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

                var obj = _spotify.GetPlayerStatus(userId);

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

        [HttpGet, Route("data/sessions"), Authorize]
        public JsonResult GetSessions([FromQuery] PagingParams paging)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var uuid = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var userId = _ctx.Users.Where(u => u.Uuid.ToString() == uuid).FirstOrDefault()?.UserId ?? -1;

                var (count, obj) = _spotify.GetAllSpotifySessions(userId, paging.Offset, paging.Limit);

                var prevOffset = paging.Limit.HasValue && paging.Offset - paging.Limit >= 0 ? paging.Offset + paging.Limit : null;
                var nextOffset = paging.Limit.HasValue && paging.Offset + paging.Limit < count ? paging.Offset + paging.Limit : null;

                return ResponseHelper.GenerateResponse(new
                {
                    TotalCount = count,
                    Sessions = obj.Select(s => new {
                        s.SessionId,
                        s.StartTime,
                        s.EndTime,
                        s.SongCount,
                        s.TimeListening
                    }).ToList(),
                    PrevOffset = prevOffset,
                    NextOffset = nextOffset,
                    Limit = paging.Limit
                }, (int)HttpStatusCode.OK);
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

        [HttpGet, Route("data/session/{id:int}"), Authorize]
        public JsonResult GetSession(int id)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var uuid = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var userId = _ctx.Users.Where(u => u.Uuid.ToString() == uuid).FirstOrDefault()?.UserId ?? -1;

                var obj = _spotify.GetSpotifySession(userId, id);

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

        [HttpGet, Route("data/songs"), Authorize]
        public JsonResult GetSongs([FromQuery] PagingParams paging)
        {
            try
            {
                var (count, obj) = _spotify.GetAllSongs(paging.Offset, paging.Limit);

                var prevOffset = paging.Limit.HasValue && paging.Offset - paging.Limit >= 0 ? paging.Offset + paging.Limit : null;
                var nextOffset = paging.Limit.HasValue && paging.Offset + paging.Limit < count ? paging.Offset + paging.Limit : null;

                return ResponseHelper.GenerateResponse(new
                {
                    TotalCount = count,
                    Songs = obj,
                    PrevOffset = prevOffset,
                    NextOffset = nextOffset,
                    Limit = paging.Limit
                }, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("data/song/{id}"), Authorize]
        public JsonResult GetSong(string id)
        {
            try
            {
                var obj = _spotify.GetSong(id);

                return ResponseHelper.GenerateResponse(obj, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("stats/songs"), Authorize]
        public JsonResult GetSongStatistics([FromQuery] PagingParams paging)
        {
            try
            {
                var (count, obj) = _spotify.GetSongStatistics(SqlDateTime.MinValue.Value, DateTime.Now, paging.Offset, paging.Limit);

                var prevOffset = paging.Limit.HasValue && paging.Offset - paging.Limit >= 0 ? paging.Offset + paging.Limit : null;
                var nextOffset = paging.Limit.HasValue && paging.Offset + paging.Limit < count ? paging.Offset + paging.Limit : null;

                return ResponseHelper.GenerateResponse(new
                {
                    TotalCount = count,
                    Statistics = obj,
                    PrevOffset = prevOffset,
                    NextOffset = nextOffset,
                    Limit = paging.Limit
                }, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("stats/artists"), Authorize]
        public JsonResult GetArtistStatistics([FromQuery] PagingParams paging)
        {
            try
            {
                var (count, obj) = _spotify.GetArtistStatistics(SqlDateTime.MinValue.Value, DateTime.Now, paging.Offset, paging.Limit);

                var prevOffset = paging.Limit.HasValue && paging.Offset - paging.Limit >= 0 ? paging.Offset + paging.Limit : null;
                var nextOffset = paging.Limit.HasValue && paging.Offset + paging.Limit < count ? paging.Offset + paging.Limit : null;

                return ResponseHelper.GenerateResponse(new
                {
                    TotalCount = count,
                    Statistics = obj,
                    PrevOffset = prevOffset,
                    NextOffset = nextOffset,
                    Limit = paging.Limit
                }, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("stats/albums"), Authorize]
        public JsonResult GetAlbumStatistics([FromQuery] PagingParams paging)
        {
            try
            {
                var (count, obj) = _spotify.GetAlbumStatistics(SqlDateTime.MinValue.Value, DateTime.Now, paging.Offset, paging.Limit);

                var prevOffset = paging.Limit.HasValue && paging.Offset - paging.Limit >= 0 ? paging.Offset + paging.Limit : null;
                var nextOffset = paging.Limit.HasValue && paging.Offset + paging.Limit < count ? paging.Offset + paging.Limit : null;

                return ResponseHelper.GenerateResponse(new
                {
                    TotalCount = count,
                    Statistics = obj,
                    PrevOffset = prevOffset,
                    NextOffset = nextOffset,
                    Limit = paging.Limit
                }, (int)HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return ResponseHelper.GenerateResponse(new { Message = e.Message }, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
