using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifySong
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int Duration { get; set; }

    public string? Url { get; set; }

    public virtual ICollection<SpotifySongAlbum> SpotifySongAlbums { get; } = new List<SpotifySongAlbum>();

    public virtual ICollection<SpotifySongArtist> SpotifySongArtists { get; } = new List<SpotifySongArtist>();

    public virtual ICollection<SpotifyTrackPlay> SpotifyTrackPlays { get; } = new List<SpotifyTrackPlay>();
}
