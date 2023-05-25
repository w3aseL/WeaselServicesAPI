using DataAccessLayer;
using DataAccessLayer.Models;
using SpotifyAPI.Web;
using SpotifyAPILibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPILibrary
{
    public class SpotifyDBLookup
    {
        private const int CODE_EXPIRATION_TIME_MIN = 30;
        private static string[] SPOTIFY_SCOPES = new []{ Scopes.UserReadEmail, Scopes.UserReadPlaybackState, Scopes.UserReadPlaybackPosition, Scopes.UserReadCurrentlyPlaying, Scopes.UserReadRecentlyPlayed, Scopes.UserTopRead };

        private readonly ServicesAPIContext _ctx;

        public SpotifyDBLookup(ServicesAPIContext ctx)
        {
            _ctx = ctx;
        }

        public SpotifyAccount GetAccountByUserId(int userId)
        {
            return _ctx.SpotifyAccounts.Where(sa => sa.UserId == userId).FirstOrDefault();
        }

        public SpotifyAccountRequest GetRequestByCode(string code)
        {
            return _ctx.SpotifyAccountRequests.Where(sar => sar.AuthorizationCode.ToString() == code).FirstOrDefault();
        }

        public LoginRequest CreateAccountRequest(int userId, string clientId, string redirectUrl)
        {
            var req = _ctx.SpotifyAccountRequests.FirstOrDefault(sar => sar.UserId == userId);

            // cleanup old request
            if (req is not null && DateTime.Now > req.ExpirationDate)
            {
                _ctx.SpotifyAccountRequests.Remove(req);
                req = null;
            }

            // generate fresh request if null
            if (req is null)
            {
                req = new SpotifyAccountRequest
                {
                    UserId = userId,
                    ExpirationDate = DateTime.Now.AddMinutes(CODE_EXPIRATION_TIME_MIN)
                };
                _ctx.SpotifyAccountRequests.Add(req);
                _ctx.SaveChanges();
            }

            return new LoginRequest(new Uri(redirectUrl), clientId, LoginRequest.ResponseType.Code)
            {
                Scope = SPOTIFY_SCOPES,
                State = req.AuthorizationCode.ToString()
            };
        }

        public async Task SetupAccountWithCode(string authCode, string authState, string clientId, string clientSecret, string redirectUrl)
        {
            var accountReq = _ctx.SpotifyAccountRequests.FirstOrDefault(sar => sar.AuthorizationCode.ToString() == authState);
            var userId = accountReq.UserId;

            var newRes = await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(clientId, clientSecret, authCode, new Uri(redirectUrl)));

            var spotifyUser = _ctx.SpotifyAccounts.FirstOrDefault(u => u.UserId == userId);

            if (spotifyUser is null)
            {
                spotifyUser = new SpotifyAccount()
                {
                    UserId = userId
                };
                _ctx.SpotifyAccounts.Add(spotifyUser);
            }

            spotifyUser.AccessToken = newRes.AccessToken;
            spotifyUser.RefreshToken = newRes.RefreshToken;
            spotifyUser.AccessGeneratedDate = newRes.CreatedAt;
            spotifyUser.RefreshGeneratedDate = newRes.CreatedAt;
            spotifyUser.ExpiresIn = newRes.ExpiresIn;
            _ctx.SpotifyAccountRequests.Remove(accountReq);
            _ctx.SaveChanges();
        }

        public async Task<string> GetAccountAccessToken(int userId, string clientId, string clientSecret)
        {
            var account = GetAccountByUserId(userId);

            if (account is null)
                throw new SpotifyAccountNotFoundException("Could not find Spotify account linked to that user!");

            if (account.AccessGeneratedDate.AddSeconds(account.ExpiresIn) < DateTime.Now)
            {
                var newResponse = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(clientId, clientSecret, account.RefreshToken));

                account.AccessToken = newResponse.AccessToken;
                account.AccessGeneratedDate = newResponse.CreatedAt;
                account.ExpiresIn = newResponse.ExpiresIn;
            }

            return account.AccessToken;
        }

        public List<SpotifySession> GetAllSpotifySessionsByUser(int userId)
        {
            var account = GetAccountByUserId(userId);

            if (account is null)
                throw new SpotifyAccountNotFoundException("Could not find Spotify account linked to that user!");

            return _ctx.SpotifySessions.Where(s => s.AccountId == account.SpotifyAuthId).ToList();
        }

        public SpotifySession GetSpotifySessionByUser(int userId, int sessionId)
        {
            var account = GetAccountByUserId(userId);

            if (account is null)
                throw new SpotifyAccountNotFoundException("Could not find Spotify account linked to that user!");

            return _ctx.SpotifySessions.FirstOrDefault(s => s.AccountId == account.SpotifyAuthId && s.Id == sessionId);
        }
    }
}
