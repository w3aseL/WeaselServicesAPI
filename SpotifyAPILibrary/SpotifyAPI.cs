using DataAccessLayer;
using DataAccessLayer.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;
using SpotifyAPILibrary.Models;
using SpotifyAPILibrary.Services;
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
        private SpotifyPlaylistService _playlistService;

        public SpotifyAPI(SpotifySettings settings, ServicesAPIContext ctx, SpotifyClientFactory clientFactory, SpotifyStateManager stateManager)
        {
            _settings = settings;
            _clientFactory = clientFactory;
            _lookup = new SpotifyDBLookup(ctx);
            _stateManager = stateManager;
            _playlistService = new SpotifyPlaylistService(ctx);
        }

        public async Task<SpotifySongModel> GetTrack()
        {
            var client = _clientFactory.CreateBasicClient();

            var track = await client.Tracks.Get("13b32GfUIo3BV93C8KtEdj");

            return new SpotifySongModel(track);
        }

        #region Account

        public Uri GetAccountRequestUrl(int userId, string redirectUrl)
        {
            var loginRequest = _lookup.CreateAccountRequest(userId, _settings.ClientId, redirectUrl);

            return loginRequest.ToUri();
        }

        public async Task CompleteAccountRequest(string authCode, string authState, string redirectUrl)
        {
            await _lookup.SetupAccountWithCode(authCode, authState, _settings.ClientId, _settings.ClientSecret, redirectUrl);
        }

        #endregion

        #region Player

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

        #endregion

        #region Sessions

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

        public List<SessionModel> GetRecentSessions(int userId)
        {
            return _lookup.GetRecentSpotifySessions(userId).Select(s => new SessionModel(s)).ToList();
        }

        #endregion

        #region Session

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

        #endregion

        #region Artists

        public List<SpotifyArtistModel> GetAllArtists()
        {
            return _lookup.GetAllSpotifyArtists().Select(artist => new SpotifyArtistModel(artist)).ToList();
        }

        public SpotifyArtistModel GetArtist(string artistId)
        {
            var artist = _lookup.GetSpotifyArtist(artistId);

            return artist != null ? new SpotifyArtistModel(artist) : new SpotifyArtistModel();
        }

        public async Task<int> UpdateArtistGenres()
        {
            var client = _clientFactory.CreateBasicClient();

            return await _lookup.UpdateArtistGenres(client);
        }

        #endregion

        #region Statistics

        public (int, List<SpotifySongStatisticModel>) GetSongStatistics(DateTime? startDate, DateTime? endDate, int offset = 0, int? limit = null)
        {
            var trackPlays = _lookup.GetTrackPlaysInRange(startDate, endDate);

            var tpQueryable = trackPlays.GroupBy(tp => new { tp.Song.Title, tp.Song.SpotifySongArtists.First().Artist.Name })
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

            var tpQueryable = trackPlays.GroupBy(tp => tp.Song.SpotifySongArtists.First().Artist.Name)
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

        public SpotifySummaryModel GetTimespanSummary(DateTime startDate, DateTime? endDate)
        {
            var sessions = _lookup.GetSessionsInRange(startDate, endDate);

            return new SpotifySummaryModel
            {
                SessionCount = sessions.Count,
                TimeListening = sessions.Sum(s => s.TimeListening),
                SkipCount = sessions.Sum(s => s.SkipCount) ?? 0,
                SongsPlayed = sessions.Sum(s => s.SpotifyTrackPlays.Count),
                UniqueSongs = sessions.SelectMany(s => s.SpotifyTrackPlays)
                    .GroupBy(tp => tp.SongId).Select(g => g.First()).Count()
            };
        }

        #endregion

        #region Playlist Testing

        public async Task<SpotifyPlaylistModel> CreateTestPlaylist(int userId, string title, string description, DateTime? startDate, int songCount=25)
        {
            var accessToken = await _lookup.GetAccountAccessToken(userId, _settings.ClientId, _settings.ClientSecret);

            var client = _clientFactory.CreateUserClient(accessToken);

            var playlist = await _playlistService.CreatePlaylistByTimespan(client, songCount, title, description, startDate.HasValue ? startDate.Value : System.Data.SqlTypes.SqlDateTime.MinValue.Value);

            return playlist != null ? new SpotifyPlaylistModel(playlist) : null;
        }

        public async Task<SpotifyPlaylistModel> GeneratePlaylist(int userId, int playlistOption, int? sessionId=null)
        {
            var accessToken = await _lookup.GetAccountAccessToken(userId, _settings.ClientId, _settings.ClientSecret);

            var client = _clientFactory.CreateUserClient(accessToken);

            var option = SpotifyPlaylistGenerationReferences.GetGenerationOptionById(playlistOption);

            if (option == null) return null;

            var now = DateTime.Now;

            if (playlistOption == 1 || playlistOption == 2)
            {
                var playlist = await _playlistService.CreatePlaylistByTimespan(client, option.SongCount, option.PlaylistTitle, $"{option.PlaylistDescription} Last generated: {now.ToString("MM/dd/yyyy hh:mm tt")}",
                 option.DateOperation != null ? option.DateOperation(now) : System.Data.SqlTypes.SqlDateTime.MinValue.Value);

                return playlist != null ? new SpotifyPlaylistModel(playlist) : null;
            }
            else if (playlistOption == 3)
            {
                if (sessionId == null) return null;

                var playlist = await _playlistService.CreatePlaylistBySession(client, userId, sessionId.Value);

                return playlist != null ? new SpotifyPlaylistModel(playlist) : null;
            }

            return null;
        }

        #endregion
    }
}
