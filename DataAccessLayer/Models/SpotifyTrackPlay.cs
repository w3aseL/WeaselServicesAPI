using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifyTrackPlay
{
    public int Id { get; set; }

    public string SongId { get; set; } = null!;

    public int SessionId { get; set; }

    public int TimePlayed { get; set; }

    public virtual SpotifySession Session { get; set; } = null!;

    public virtual SpotifySong Song { get; set; } = null!;
}
