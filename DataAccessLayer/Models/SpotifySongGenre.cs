using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifySongGenre
{
    public int Id { get; set; }

    public string SongId { get; set; } = null!;

    public int GenreId { get; set; }

    public virtual SpotifyGenre Genre { get; set; } = null!;

    public virtual SpotifySong Song { get; set; } = null!;
}
