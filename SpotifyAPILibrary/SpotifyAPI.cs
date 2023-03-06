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
    }
}
