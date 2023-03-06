using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifySongArtist
{
    public int Id { get; set; }

    public string SongId { get; set; } = null!;

    public string ArtistId { get; set; } = null!;

    public virtual SpotifyArtist Artist { get; set; } = null!;

    public virtual SpotifySong Song { get; set; } = null!;
}
