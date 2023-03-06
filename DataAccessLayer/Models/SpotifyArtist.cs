using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifyArtist
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Url { get; set; }

    public virtual ICollection<SpotifyArtistAlbum> SpotifyArtistAlbums { get; } = new List<SpotifyArtistAlbum>();

    public virtual ICollection<SpotifySongArtist> SpotifySongArtists { get; } = new List<SpotifySongArtist>();
}
