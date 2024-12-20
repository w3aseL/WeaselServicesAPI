﻿using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web;
using SpotifyAPILibrary.Exceptions;
using SpotifyAPILibrary.Models;
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
        private static string[] SPOTIFY_SCOPES = new []{ Scopes.UserReadEmail, Scopes.UserReadPrivate, Scopes.UserReadPlaybackState, Scopes.UserReadPlaybackPosition, Scopes.UserReadCurrentlyPlaying, Scopes.UserReadRecentlyPlayed, Scopes.UserTopRead, Scopes.PlaylistModifyPublic, Scopes.PlaylistModifyPrivate, Scopes.PlaylistReadCollaborative, Scopes.PlaylistReadPrivate };

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
            Guid? state = null;

            if (Guid.TryParse(authState, out Guid guid)) state = guid;

            var accountReq = _ctx.SpotifyAccountRequests.FirstOrDefault(sar => sar.AuthorizationCode == state);
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

        public (int, List<SpotifySession>) GetAllSpotifySessionsByUser(int userId, int offset=0, int? limit=null)
        {
            var account = GetAccountByUserId(userId);

            if (account is null)
                throw new SpotifyAccountNotFoundException("Could not find Spotify account linked to that user!");

            var totalNumSessions = _ctx.SpotifySessions.Count();

            var sessions = _ctx.SpotifySessions
                .Where(s => s.AccountId == account.SpotifyAuthId)
                .Skip(offset);

            if (limit.HasValue) sessions = sessions.Take(limit.Value);

            return (totalNumSessions, sessions.ToList());
        }

        public SpotifySession GetSpotifySessionByUser(int userId, int sessionId)
        {
            var account = GetAccountByUserId(userId);

            if (account is null)
                throw new SpotifyAccountNotFoundException("Could not find Spotify account linked to that user!");

            return _ctx.SpotifySessions.Where(s => s.AccountId == account.SpotifyAuthId && s.Id == sessionId)
                .Include(s => s.SpotifyTrackPlays)
                .ThenInclude(s => s.Song)
                .ThenInclude(s => s.SpotifySongArtists)
                .ThenInclude(sa => sa.Artist)
                .Include(s => s.SpotifyTrackPlays)
                .ThenInclude(s => s.Song)
                .ThenInclude(s => s.SpotifySongAlbums)
                .ThenInclude(sa => sa.Album)
                .ThenInclude(a => a.SpotifyArtistAlbums)
                .ThenInclude(aa => aa.Artist)
                .FirstOrDefault();
        }

        public List<SpotifySession> GetRecentSpotifySessions(int userId, int count=3)
        {
            var account = GetAccountByUserId(userId);

            if (account is null)
                throw new SpotifyAccountNotFoundException("Could not find Spotify account linked to that user!");

            return _ctx.SpotifySessions.Where(s => s.AccountId == account.SpotifyAuthId)
                .OrderByDescending(s => s.Id)
                .Take(count)
                .ToList();
        }

        public (int, List<SpotifySong>) GetAllSpotifySongs(int offset=0, int? limit=null)
        {
            var count = _ctx.SpotifySongs.Count();

            var pagedThrough = _ctx.SpotifySongs.Skip(offset);

            if (limit.HasValue) pagedThrough = pagedThrough.Take(limit.Value);

            return (count, pagedThrough
                .Include(s => s.SpotifySongArtists)
                .Include(s => s.SpotifySongArtists)
                .ThenInclude(sa => sa.Artist)
                .ThenInclude(a => a.SpotifyArtistGenres)
                .ThenInclude(a => a.Genre)
                .Include(s => s.SpotifySongAlbums)
                .ThenInclude(sa => sa.Album)
                .ThenInclude(a => a.SpotifyArtistAlbums)
                .ThenInclude(aa => aa.Artist)
                .ToList());
        }

        public SpotifySong GetSpotifySong(string songId)
        {
            return _ctx.SpotifySongs.Where(s => s.Id == songId)
                .Include(s => s.SpotifySongArtists)
                .Include(s => s.SpotifySongArtists)
                .ThenInclude(sa => sa.Artist)
                .ThenInclude(a => a.SpotifyArtistGenres)
                .ThenInclude(a => a.Genre)
                .Include(s => s.SpotifySongAlbums)
                .ThenInclude(sa => sa.Album)
                .ThenInclude(a => a.SpotifyArtistAlbums)
                .ThenInclude(aa => aa.Artist)
                .FirstOrDefault(s => s.Id == songId);
        }

        public List<SpotifyArtist> GetAllSpotifyArtists(int offset=0, int? limit=null)
        {
            var pagedThrough = _ctx.SpotifyArtists.Skip(offset);

            if (limit.HasValue) pagedThrough = pagedThrough.Take(limit.Value);

            return pagedThrough.ToList();
        }

        public SpotifyArtist GetSpotifyArtist(string artistId)
        {
            return _ctx.SpotifyArtists
                .Include(sa => sa.SpotifyArtistGenres)
                .ThenInclude(ag => ag.Genre)
                .FirstOrDefault(s => s.Id == artistId);
        }

        public async Task<int> UpdateArtistGenres(SpotifyClient client)
        {
            var artistsWithNoGenre = _ctx.SpotifyArtists.Where(a => !a.SpotifyArtistGenres.Any()).ToList();
            var artistIds = artistsWithNoGenre.Select(a => a.Id).ToList();
            var artistsUpdated = 0;

            var existingGenres = _ctx.SpotifyGenres.ToList();

            using (var tx = _ctx.Database.BeginTransaction())
            {
                try
                {
                    for (var offset = 0; offset < artistIds.Count; offset += 50)
                    {
                        var limit = offset + 50 > artistIds.Count ? artistIds.Count - offset : 50;

                        var artistsToReceive = artistIds.GetRange(offset, limit);

                        var fullArtists = await client.Artists.GetSeveral(new ArtistsRequest(artistsToReceive));

                        foreach (var artist in fullArtists.Artists)
                        {
                            // update genres
                            foreach (var genre in artist.Genres)
                            {
                                var existingGenre = existingGenres.FirstOrDefault(g => g.Name == genre);

                                if (existingGenre is null)
                                {
                                    existingGenre = new SpotifyGenre
                                    {
                                        Name = genre
                                    };
                                    _ctx.SpotifyGenres.Add(existingGenre);
                                    _ctx.SaveChanges();

                                    // save genre to existing list to prevent duplicates
                                    existingGenres.Add(existingGenre);
                                }

                                _ctx.SpotifyArtistGenres.Add(new SpotifyArtistGenre
                                {
                                    ArtistId = artist.Id,
                                    GenreId = existingGenre.Id
                                });
                                _ctx.SaveChanges();
                            }

                            artistsUpdated += 1;
                        }
                    }

                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw ex;
                }
            }

            return artistsUpdated;
        }

        public IQueryable<SpotifyTrackPlay> GetTrackPlaysInRange(DateTime? startDate, DateTime? endDate)
        {
            return _ctx.SpotifyTrackPlays
                    .Where(tp => (tp.Session.StartTime >= startDate && tp.Session.StartTime <= endDate)
                  || (tp.Session.EndTime >= startDate && tp.Session.EndTime <= endDate))
                    .Include(tp => tp.Song)
                    .Include(tp => tp.Song.SpotifySongArtists)
                    .ThenInclude(sa => sa.Artist)
                    .ThenInclude(a => a.SpotifyArtistGenres)
                    .ThenInclude(a => a.Genre)
                    .Include(tp => tp.Song.SpotifySongAlbums)
                    .ThenInclude(sa => sa.Album)
                    .ThenInclude(a => a.SpotifyArtistAlbums)
                    .ThenInclude(aa => aa.Artist);
        }

        public List<SpotifySession> GetSessionsInRange(DateTime? startDate, DateTime? endDate)
        {
            return _ctx.SpotifySessions
                .Where(s => (s.StartTime >= startDate && s.StartTime <= endDate)
                  || (s.EndTime >= startDate && s.EndTime <= endDate))
                .Include(s => s.SpotifyTrackPlays)
                .ToList();
        }
    }
}
