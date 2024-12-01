using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifyArtistGenre
{
    public int Id { get; set; }

    public string ArtistId { get; set; } = null!;

    public int GenreId { get; set; }

    public virtual SpotifyArtist Artist { get; set; } = null!;

    public virtual SpotifyGenre Genre { get; set; } = null!;
}
