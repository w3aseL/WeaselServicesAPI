using DataAccessLayer;
using DataAccessLayer.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;
using SpotifyAPILibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPILibrary
{
    public class SpotifyAPI : ISpotifyAPI
    {
        private readonly SpotifySettings _settings;
        private SpotifyClientFactory _clientFactory;
        private SpotifyDBLookup _lookup;
        private SpotifyStateManager _stateManager;

        public SpotifyAPI(SpotifySettings settings, ServicesAPIContext ctx, SpotifyClientFactory clientFactory, SpotifyStateManager stateManager)
        {
            _settings = settings;
            _clientFactory = clientFactory;
            _lookup = new SpotifyDBLookup(ctx);
            _stateManager = stateManager;
        }

        public async Task<SpotifySongModel> GetTrack()
        {
            var client = _clientFactory.CreateBasicClient();

            var track = await client.Tracks.Get("13b32GfUIo3BV93C8KtEdj");

            return new SpotifySongModel(track);
        }

        public Uri GetAccountRequestUrl(int userId, string redirectUrl)
        {
            var loginRequest = _lookup.CreateAccountRequest(userId, _settings.ClientId, redirectUrl);

            return loginRequest.ToUri();
        }

        public async Task CompleteAccountRequest(string authCode, string authState, string redirectUrl)
        {
            await _lookup.SetupAccountWithCode(authCode, authState, _settings.ClientId, _settings.ClientSecret, redirectUrl);
        }

        public SpotifySongModel GetCurrentlyListenedToSong(int userId)
        {
            var accountId = _lookup.GetAccountByUserId(userId).SpotifyAuthId;

            var listeningData = _stateManager.GetActiveState(accountId);

            // gambling that it is a track
            if (listeningData?.IsPlaying == true)
                return listeningData.CurrentSong;

            return null;
        }

        public SpotifyPlayerStateModel GetPlayerStatus(int userId)
        {
            var accountId = _lookup.GetAccountByUserId(userId).SpotifyAuthId;

            return _stateManager.GetActiveState(accountId).SerializePlayerState();
        }

        public (int, List<SessionModel>) GetAllSpotifySessions(int userId, int offset=0, int? limit=null)
        {
            var (total, sessions) = _lookup.GetAllSpotifySessionsByUser(userId, offset, limit);

            return (total, sessions.Select(s => new SessionModel(s)).ToList());
        }

        public SessionModel GetSpotifySession(int userId, int sessionId)
        {
            var session = _lookup.GetSpotifySessionByUser(userId, sessionId);

            if (session is null)
                throw new ArgumentNullException("Could not find session with session ID provided!");

            return new SessionModel(session)
            {
                TrackPlays = session.SpotifyTrackPlays.Select(tp => new TrackPlayModel(tp)).ToList()
            };
        }

        public (int, List<SpotifySongModel>) GetAllSongs(int offset = 0, int? limit = null)
        {
            var (total, songs) = _lookup.GetAllSpotifySongs(offset, limit);

            return (total, songs.Select(song => new SpotifySongModel(song)).ToList());
        }

        public SpotifySongModel GetSong(string songId)
        {
            var song = _lookup.GetSpotifySong(songId);

            return song != null ? new SpotifySongModel(song) : new SpotifySongModel();
        }

        public List<SpotifyArtistModel> GetAllArtists()
        {
            return _lookup.GetAllSpotifyArtists().Select(artist => new SpotifyArtistModel(artist)).ToList();
        }

        public SpotifyArtistModel GetArtist(string artistId)
        {
            var artist = _lookup.GetSpotifyArtist(artistId);

            return artist != null ? new SpotifyArtistModel(artist) : new SpotifyArtistModel();
        }

        public (int, List<SpotifySongStatisticModel>) GetSongStatistics(DateTime? startDate, DateTime? endDate, int offset = 0, int? limit = null)
        {
            var trackPlays = _lookup.GetTrackPlaysInRange(startDate, endDate);

            var tpQueryable = trackPlays.GroupBy(tp => tp.Song.Title)
                .Select(g => new SpotifySongStatisticModel
                {
                    TimesPlayed = g.Count(),
                    TimeListening = g.Sum(s => s.TimePlayed),
                    Song = new SpotifySongModel(g.First().Song)
                })
                .OrderByDescending(m => m.TimesPlayed)
                .ThenByDescending(m => m.TimeListening);

            var count = tpQueryable.Count();

            var queryable = tpQueryable
                .Skip(offset);

            if (limit.HasValue) queryable = queryable.Take(limit.Value);

            return (count, queryable.ToList());
        }

        public (int, List<SpotifyArtistStatisticModel>) GetArtistStatistics(DateTime? startDate, DateTime? endDate, int offset = 0, int? limit = null)
        {
            var trackPlays = _lookup.GetTrackPlaysInRange(startDate, endDate).ToList();

            var tpQueryable = trackPlays.GroupBy(tp => tp.Song.SpotifySongArtists.First().Artist)
                .Select(g => new SpotifyArtistStatisticModel
                {
                    TimesPlayed = g.Count(),
                    TimeListening = g.Sum(s => s.TimePlayed),
                    Artist = new SpotifyArtistModel(g.First().Song.SpotifySongArtists.First().Artist)
                })
                .OrderByDescending(m => m.TimesPlayed)
                .ThenByDescending(m => m.TimeListening);

            var count = tpQueryable.Count();

            var queryable = tpQueryable
                .Skip(offset);

            if (limit.HasValue) queryable = queryable.Take(limit.Value);

            return (count, queryable.ToList());
        }

        public (int, List<SpotifyAlbumStatisticModel>) GetAlbumStatistics(DateTime? startDate, DateTime? endDate, int offset = 0, int? limit = null)
        {
            var trackPlays = _lookup.GetTrackPlaysInRange(startDate, endDate).ToList();

            var tpQueryable = trackPlays.Where(tp => tp.Song.SpotifySongAlbums.Any()).GroupBy(tp => tp.Song.SpotifySongAlbums.First().Album.Title)
                .Select(g => new SpotifyAlbumStatisticModel
                {
                    TimesPlayed = g.Count(),
                    TimeListening = g.Sum(s => s.TimePlayed),
                    Album = new SpotifyAlbumModel(g.First().Song.SpotifySongAlbums.First().Album)
                })
                .OrderByDescending(m => m.TimesPlayed)
                .ThenByDescending(m => m.TimeListening);

            var count = tpQueryable.Count();

            var queryable = tpQueryable
                .Skip(offset);

            if (limit.HasValue) queryable = queryable.Take(limit.Value);

            return (count, queryable.ToList());
        }
    }
}
