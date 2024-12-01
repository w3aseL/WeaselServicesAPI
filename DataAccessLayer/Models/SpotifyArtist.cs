using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifyArtist
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Url { get; set; }

    public virtual ICollection<SpotifyArtistAlbum> SpotifyArtistAlbums { get; set; } = new List<SpotifyArtistAlbum>();

    public virtual ICollection<SpotifyArtistGenre> SpotifyArtistGenres { get; set; } = new List<SpotifyArtistGenre>();

    public virtual ICollection<SpotifySongArtist> SpotifySongArtists { get; set; } = new List<SpotifySongArtist>();
}
