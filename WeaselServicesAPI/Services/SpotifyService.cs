using SpotifyAPI.Web;
using SpotifyAPILibrary;
using SpotifyAPILibrary.Models;

namespace WeaselServicesAPI.Services
{
    public class SpotifyService
    {
        private readonly ISpotifyAPI _spotify;

        public SpotifyService(ISpotifyAPI spotify)
        {
            _spotify = spotify;
        }

        public async Task<SpotifySongModel> GetTestTrack()
        {
            return await _spotify.GetTrack();
        }

        public Uri GetNewAccountUri(int userId, string redirectUrl)
        {
            return _spotify.GetAccountRequestUrl(userId, redirectUrl);
        }

        public async Task ConfirmAccountWithCode(string code, string state, string redirectUrl)
        {
            await _spotify.CompleteAccountRequest(code, state, redirectUrl);
        }

        public SpotifySongModel GetCurrentlyPlayedSong(int userId)
        {
            return _spotify.GetCurrentlyListenedToSong(userId);
        }

        public SpotifyPlayerStateModel GetPlayerStatus(int userId)
        {
            return _spotify.GetPlayerStatus(userId);
        }
    }
}
