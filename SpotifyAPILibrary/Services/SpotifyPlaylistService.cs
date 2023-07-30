using DataAccessLayer;
using SpotifyAPI.Web;
using SpotifyAPILibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPILibrary.Services
{
    public sealed class SpotifyPlaylistService
    {
        private readonly SpotifyDBLookup _dbLookup;

        public SpotifyPlaylistService(ServicesAPIContext ctx)
        {
            _dbLookup = new SpotifyDBLookup(ctx);
        }

        public async Task<FullPlaylist> CreatePlaylistByTimespan(SpotifyClient client, int songCount, string title, string description, DateTime startInterval)
        {
            var userId = (await client.UserProfile.Current()).Id;
            var existingPlaylist = await SearchForPlaylist(client, userId, title);

            var now = DateTime.Now;

            var tracks = GetTopTracks(startInterval, now, songCount);

            if (existingPlaylist == null)
            {
                var newPlaylist = await client.Playlists.Create(userId, new PlaylistCreateRequest(title)
                {
                    Description = description
                });

                var success = await client.Playlists.ReplaceItems(newPlaylist.Id, new PlaylistReplaceItemsRequest(tracks));

                if (success) return newPlaylist;
            }
            else
            {
                var fullPlaylist = await client.Playlists.Get(existingPlaylist.Id);

                // update details
                var success = await client.Playlists.ChangeDetails(fullPlaylist.Id, new PlaylistChangeDetailsRequest
                {
                    Description = description
                });

                success = await client.Playlists.ReplaceItems(fullPlaylist.Id, new PlaylistReplaceItemsRequest(tracks));

                if (success) return fullPlaylist;
            }

            return null;
        }

        private async Task<SimplePlaylist> SearchForPlaylist(SpotifyClient client, string userId, string title)
        {
            var paging = await client.Playlists.GetUsers(userId);

            await foreach (var item in client.Paginate(paging))
            {
                if (item.Name == title) return item;
            }

            return null;
        }

        private List<string> GetTopTracks(DateTime startInterval, DateTime endInterval, int songCount)
        {
            return _dbLookup.GetTrackPlaysInRange(startInterval, endInterval)
                .GroupBy(tp => new { tp.Song.Title, tp.Song.SpotifySongArtists.First().Artist.Name })
                .Select(g => new SpotifySongStatisticModel
                {
                    TimesPlayed = g.Count(),
                    TimeListening = g.Sum(s => s.TimePlayed),
                    Song = new SpotifySongModel(g.First().Song)
                })
                .OrderByDescending(m => m.TimesPlayed)
                .ThenByDescending(m => m.TimeListening)
                .ToList()
                .Take(songCount)
                .Select(g => string.Format("spotify:track:{0}", g.Song.Id))
                .ToList();
        }
    }
}
