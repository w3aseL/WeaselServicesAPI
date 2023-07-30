using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifyAlbum
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Url { get; set; }

    public string? ArtworkUrl { get; set; }

    public virtual ICollection<SpotifyArtistAlbum> SpotifyArtistAlbums { get; set; } = new List<SpotifyArtistAlbum>();

    public virtual ICollection<SpotifySongAlbum> SpotifySongAlbums { get; set; } = new List<SpotifySongAlbum>();
}
