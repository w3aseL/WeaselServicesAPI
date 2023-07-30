using DataAccessLayer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPILibrary.Services
{
    public sealed class SpotifyPlaylistBackgroundService : BackgroundService
    {
        private readonly SpotifySettings _settings;
        private readonly SpotifyClientFactory _clientFactory;
        private readonly IServiceProvider _serviceProvider;
        private ILogger<SpotifyPlaylistBackgroundService> _logger;

        public SpotifyPlaylistBackgroundService(IServiceProvider serviceProvider, ILogger<SpotifyPlaylistBackgroundService> logger, SpotifySettings settings, SpotifyClientFactory factory)
        {
            _settings = settings;
            _clientFactory = factory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (IsTimeToRunTask()) await RunPlaylistRefreshes();

                var timespan = CalculateTimeToNextRun();

                _logger.LogInformation($"Time to playlist refresh: {timespan.ToString()} ");

                await Task.Delay(timespan, stoppingToken);
            }
        }

        private async Task RunPlaylistRefreshes()
        {
            using IServiceScope scope = _serviceProvider.CreateAsyncScope();

            var ctx = scope.ServiceProvider.GetRequiredService<ServicesAPIContext>();

            var playlistService = new SpotifyPlaylistService(ctx);

            var accounts = ctx.SpotifyAccounts.ToList();

            foreach (var account in accounts)
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

                var now = DateTime.Now;

                // generate all-time playlist
                var allTimePlaylist = await playlistService.CreatePlaylistByTimespan(client, 100,
                    "Personal Top Listens (All-Time)",
                    $"An API generated playlist with 100 of my top listened songs tracked by my personal API. Last generated: { now.ToString("MM/dd/yyyy hh:mm tt") }",
                    System.Data.SqlTypes.SqlDateTime.MinValue.Value);

                var threeMonthPlaylist = await playlistService.CreatePlaylistByTimespan(client, 40,
                    "Personal Top Listens (3 Months)",
                    $"An API generated playlist with 40 of my top listened songs from the past 3 months tracked by my personal API. Last generated: {now.ToString("MM/dd/yyyy hh:mm tt")}",
                    now.AddMonths(-3));

                _logger.LogInformation($"Generated playlists at {now.ToString("MM/dd/yyyy hh:mm tt")} -- All-Time Playlist: \"{allTimePlaylist.Uri}\", 3 Month Playlist: \"{threeMonthPlaylist.Uri}\"  ");
            }
        }

        private bool IsTimeToRunTask()
        {
            var now = DateTime.Now;
            var lastDayOfMonth = DateTime.DaysInMonth(now.Year, now.Month);

            return now.Hour == 0 && now.Minute == 0 && (now.Day == lastDayOfMonth || now.Day == 15);
        }

        private TimeSpan CalculateTimeToNextRun()
        {
            var now = DateTime.Now;
            var lastDayOfMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var isLastDay = now.Day == lastDayOfMonth;

            var nextDay = isLastDay
                ? new DateTime(now.Year, now.Month, 15).AddMonths(1)
                : new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);

            return nextDay - now;
        }

        /*
        private bool IsTimeToRunTask_DEV()
        {
            var now = DateTime.Now;

            return now.Minute % 15 == 0;
        }

        private TimeSpan CalculateTimeToNextRun_DEV()
        {
            var now = DateTime.Now;

            var minToInterval = 15 - (now.Minute % 15);
            var hourOverflow = (now.Minute + minToInterval) >= 60;
            var nextDay = new DateTime(now.Year, now.Month, now.Day, now.Hour + (hourOverflow ? 1 : 0), (now.Minute + minToInterval) % 60, 0);

            return nextDay - now;
        }
        */
    }

}
