using DataAccessLayer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;
using Swan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketService;

namespace SpotifyAPILibrary
{
    public class SpotifyBackgroundTaskService : BackgroundService
    {
        private readonly SpotifySettings _settings;
        private readonly SpotifyClientFactory _clientFactory;
        private readonly TimeSpan timespan = TimeSpan.FromSeconds(1);
        private readonly IServiceProvider _serviceProvider;
        private ILogger<SpotifyBackgroundTaskService> _logger;
        private SpotifyStateManager _manager;
        private ConnectionManager _connectionManager;

        public SpotifyBackgroundTaskService(IServiceProvider serviceProvider, ILogger<SpotifyBackgroundTaskService> logger, SpotifySettings settings, SpotifyClientFactory factory, SpotifyStateManager manager, ConnectionManager connManager)
        {
            _settings = settings;
            _clientFactory = factory;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _manager = manager;
            _connectionManager = connManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(timespan);
            using IServiceScope scope = _serviceProvider.CreateAsyncScope();

            var ctx = scope.ServiceProvider.GetRequiredService<ServicesAPIContext>();

            var accounts = ctx.SpotifyAccounts.ToList();

            while(!stoppingToken.IsCancellationRequested &&
                await timer.WaitForNextTickAsync(stoppingToken))
            {
                foreach (var account in accounts)
                {
                    try
                    {
                        // refresh token if applicable
                        if (account.AccessGeneratedDate.AddSeconds(account.ExpiresIn) < DateTime.UtcNow)
                        {
                            var newResponse = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(_settings.ClientId, _settings.ClientSecret, account.RefreshToken));

                            account.AccessToken = newResponse.AccessToken;
                            account.AccessGeneratedDate = newResponse.CreatedAt;
                            account.ExpiresIn = newResponse.ExpiresIn;
                            await ctx.SaveChangesAsync();
                        }

                        // get client
                        var client = _clientFactory.CreateUserClient(account.AccessToken);

                        // testing
                        var currentlyPlaying = await client.Player.GetCurrentPlayback();
                        var (stateUpdates, stateUpdateMessage) = _manager.UpdateActiveState(account.SpotifyAuthId, currentlyPlaying);

                        if (stateUpdates)
                        {
                            _logger.LogInformation(stateUpdateMessage, DateTime.Now);

                            // emit active state
                            var currentState = _manager.GetActiveState(account.SpotifyAuthId);
                            await _connectionManager.EmitEvent($"update:player-status:{account.SpotifyAuthId}", currentState);
                        }
                    } catch(Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }
            }
        }
    }
}
