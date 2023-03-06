using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static Swan.Terminal;

namespace SpotifyAPILibrary
{
    public class SpotifyClientFactory
    {
        private SpotifyClientConfig _config;

        public SpotifyClientFactory(SpotifySettings settings)
        {
            _config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(settings.ClientId, settings.ClientSecret)); ;
        }

        public SpotifyClient CreateBasicClient()
        {
            return new SpotifyClient(_config);
        }

        public SpotifyClient CreateUserClient(string accessToken)
        {
            return new SpotifyClient(_config.WithToken(accessToken));
        }
    }
}
