using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifyArtistAlbum
{
    public int Id { get; set; }

    public string ArtistId { get; set; } = null!;

    public string AlbumId { get; set; } = null!;

    public virtual SpotifyAlbum Album { get; set; } = null!;

    public virtual SpotifyArtist Artist { get; set; } = null!;
}
