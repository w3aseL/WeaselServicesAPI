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

        public SpotifySongModel() { }

        public SpotifySongModel(FullTrack tr)
        {
            Id = tr.Id;
            Name = tr.Name;
            DurationMs = tr.DurationMs;
            Uri = tr.Uri;
            Artists = tr.Artists.Select(a => new SpotifyArtistModel(a)).ToList();
            Album = new SpotifyAlbumModel(tr.Album);
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
            Uri = a.Uri;
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
            Uri = a.Uri;
            ArtworkURL = a.Images.FirstOrDefault()?.Url ?? "";
            Artists = a.Artists.Select(a => new SpotifyArtistModel(a)).ToList();
        }
    }
}
