using DataAccessLayer.Models;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPILibrary.Models
{
    public class SpotifySongModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int DurationMs { get; set; }
        public string Uri { get; set; }
        public List<SpotifyArtistModel> Artists { get; set; }
        public SpotifyAlbumModel Album { get; set; }
        public List<SpotifyAlbumModel> Albums { get; set; }

        public SpotifySongModel() { }

        public SpotifySongModel(FullTrack tr)
        {
            Id = tr.Id;
            Name = tr.Name;
            DurationMs = tr.DurationMs;
            Uri = SpotifyUriTranslator.ConvertUriToHref(tr.Uri);
            Artists = tr.Artists.Select(a => new SpotifyArtistModel(a)).ToList();
            Album = new SpotifyAlbumModel(tr.Album);
        }

        public SpotifySongModel(SpotifySong s)
        {
            Id = s.Id;
            Name = s.Title;
            DurationMs = s.Duration;
            Uri = s.Url;
            Artists = s.SpotifySongArtists.Select(sa => new SpotifyArtistModel(sa.Artist)).ToList();
            Albums = s.SpotifySongAlbums.Select(sa => new SpotifyAlbumModel(sa.Album)).ToList();
        }
    }

    public class SpotifyArtistModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }

        public SpotifyArtistModel() { }

        public SpotifyArtistModel(SimpleArtist a)
        {
            Id = a.Id;
            Name = a.Name;
            Uri = SpotifyUriTranslator.ConvertUriToHref(a.Uri);
        }

        public SpotifyArtistModel(SpotifyArtist a)
        {
            Id = a.Id;
            Name = a.Name;
            Uri = a.Url;
        }
    }

    public class SpotifyAlbumModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string ArtworkURL { get; set; }
        public List<SpotifyArtistModel> Artists { get; set; }

        public SpotifyAlbumModel() { }

        public SpotifyAlbumModel(SimpleAlbum a)
        {
            Id = a.Id;
            Name = a.Name;
            Uri = SpotifyUriTranslator.ConvertUriToHref(a.Uri);
            ArtworkURL = a.Images.FirstOrDefault()?.Url ?? "";
            Artists = a.Artists.Select(a => new SpotifyArtistModel(a)).ToList();
        }

        public SpotifyAlbumModel(SpotifyAlbum a)
        {
            Id = a.Id;
            Name = a.Title;
            Uri = a.Url;
            ArtworkURL = a.ArtworkUrl;
            Artists = a.SpotifyArtistAlbums.Select(aa => new SpotifyArtistModel(aa.Artist)).ToList();
        }
    }

    public class TrackPlayModel
    {
        public SpotifySongModel Song { get; set; }
        public int TimePlayed { get; set; }

        public TrackPlayModel(SpotifyTrackPlay tp)
        {
            Song = new SpotifySongModel(tp.Song);
            TimePlayed = tp.TimePlayed;
        }
    }
    
    public class SessionModel
    {
        public int SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int SongCount { get; set; }
        public int TimeListening { get; set; }
        public int? SkipCount { get; set; }

        public List<TrackPlayModel> TrackPlays { get; set; }

        public SessionModel(SpotifySession s)
        {
            SessionId = s.Id;
            StartTime = s.StartTime.ToCentralTime();
            EndTime = s.EndTime.ToCentralTime();
            SongCount = s.SongCount;
            TimeListening = s.TimeListening;
            SkipCount = s.SkipCount;
        }
    }

    public class SpotifyStatisticModel
    {
        public int TimesPlayed { get; set; }
        public int TimeListening { get; set; }
    }

    public class SpotifySongStatisticModel : SpotifyStatisticModel
    {
        public SpotifySongModel Song { get; set; }
    }

    public class SpotifyArtistStatisticModel : SpotifyStatisticModel
    {
        public SpotifyArtistModel Artist { get; set; }
    }

    public class SpotifyAlbumStatisticModel : SpotifyStatisticModel
    {
        public SpotifyAlbumModel Album { get; set; }
    }

    public class SpotifyPlaylistModel
    {
        public string Uri { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public SpotifyPlaylistModel(FullPlaylist pl)
        {
            Uri = pl.Uri;
            Title = pl.Name;
            Description = pl.Description;
        }
    }

    public sealed class SpotifySummaryModel
    {
        public int TimeListening { get; set; }
        public int SkipCount { get; set; }
        public int SongsPlayed { get; set; }
        public int UniqueSongs { get; set; }
        public int SessionCount { get; set; }
    }

    public sealed class SpotifyPlaylistGenerationModel
    {
        public string PlaylistTitle { get; set; }
        public string PlaylistDescription { get; set; }
        public int SongCount { get; set; }
        public Func<DateTime, DateTime> DateOperation { get; set; }

        public SpotifyPlaylistGenerationModel() { }

        public SpotifyPlaylistGenerationModel(string playlistTitle, string playlistDescription, int songCount)
        {
            PlaylistTitle = playlistTitle;
            PlaylistDescription = playlistDescription;
            SongCount = songCount;
        }

        public SpotifyPlaylistGenerationModel(string playlistTitle, string playlistDescription, int songCount, Func<DateTime, DateTime> dateOp)
        {
            PlaylistTitle = playlistTitle;
            PlaylistDescription = playlistDescription;
            SongCount = songCount;
            DateOperation = dateOp;  
        }
    }

    public static class SpotifyPlaylistGenerationReferences
    {
        private static List<SpotifyPlaylistGenerationModel> GenerationOptions = new List<SpotifyPlaylistGenerationModel>
        {
            new SpotifyPlaylistGenerationModel("Personal Top Listens (All-Time)", "An API generated playlist with 100 of my top listened songs tracked by my personal API.", 100),
            new SpotifyPlaylistGenerationModel("Personal Top Listens (3 Months)", "An API generated playlist with 40 of my top listened songs from the past 3 months tracked by my personal API.", 40, endDate => endDate.AddMonths(-3))
        };

        public static List<SpotifyPlaylistGenerationModel> GetGenerationOptions()
        {
            return GenerationOptions;
        }

        public static SpotifyPlaylistGenerationModel GetGenerationOptionById(int option)
        {
            return option > GenerationOptions.Count - 1 || option < 0 ? null : GenerationOptions[option];
        }
    }

    public static class SpotifyUriTranslator
    {
        public static string ConvertUriToHref(string uri)
        {
            var uriSplit = uri.Split(":");

            if (uriSplit.Length != 3)
                return "https://spotify.com/";

            return $"https://open.spotify.com/{uriSplit[1]}/{uriSplit[2]}";
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime ToCentralTime(this DateTime time)
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(time, cstZone);
        }
    }
}
