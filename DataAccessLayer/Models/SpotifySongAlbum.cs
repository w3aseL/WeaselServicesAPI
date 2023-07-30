using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifySongAlbum
{
    public int Id { get; set; }

    public string SongId { get; set; } = null!;

    public string AlbumId { get; set; } = null!;

    public virtual SpotifyAlbum Album { get; set; } = null!;

    public virtual SpotifySong Song { get; set; } = null!;
}
