using DataAccessLayer.Models;
using SpotifyAPILibrary.Models;

namespace SpotifyAPILibrary
{
    public interface ISpotifyAPI
    {
        public Task<SpotifySongModel> GetTrack();
        public Uri GetAccountRequestUrl(int userId, string redirectUrl);
        public Task CompleteAccountRequest(string authCode, string authState, string redirectUrl);
        public SpotifySongModel GetCurrentlyListenedToSong(int userId);
        public SpotifyPlayerStateModel GetPlayerStatus(int userId);
        public (int, List<SessionModel>) GetAllSpotifySessions(int userId, int offset=0, int? limit=null);
        public SessionModel GetSpotifySession(int userId, int sessionId);
        public (int, List<SpotifySongModel>) GetAllSongs(int offset = 0, int? limit = null);
        public SpotifySongModel GetSong(string songId);
        public List<SpotifyArtistModel> GetAllArtists();
        public SpotifyArtistModel GetArtist(string artistId);
        public (int, List<SpotifySongStatisticModel>) GetSongStatistics(DateTime? startDate, DateTime? endDate, int offset = 0, int? limit = null);
        public (int, List<SpotifyArtistStatisticModel>) GetArtistStatistics(DateTime? startDate, DateTime? endDate, int offset = 0, int? limit = null);
        public (int, List<SpotifyAlbumStatisticModel>) GetAlbumStatistics(DateTime? startDate, DateTime? endDate, int offset = 0, int? limit = null);


    }
}
