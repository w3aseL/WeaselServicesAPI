using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifyGenre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<SpotifyArtistGenre> SpotifyArtistGenres { get; set; } = new List<SpotifyArtistGenre>();
}
