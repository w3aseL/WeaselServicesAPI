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
        public List<SessionModel> GetAllSpotifySessions(int userId);
        public SessionModel GetSpotifySession(int userId, int sessionId);
    }
}
