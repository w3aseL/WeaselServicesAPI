using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifySession
{
    public int Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int SongCount { get; set; }

    public int TimeListening { get; set; }

    public int? SkipCount { get; set; }

    public int? AccountId { get; set; }

    public virtual SpotifyAccount? Account { get; set; }

    public virtual ICollection<SpotifyTrackPlay> SpotifyTrackPlays { get; set; } = new List<SpotifyTrackPlay>();
}
