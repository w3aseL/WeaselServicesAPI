using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifyAccount
{
    public int SpotifyAuthId { get; set; }

    public int UserId { get; set; }

    public string AccessToken { get; set; } = null!;

    public DateTime AccessGeneratedDate { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime RefreshGeneratedDate { get; set; }

    public int ExpiresIn { get; set; }

    public virtual ICollection<SpotifySession> SpotifySessions { get; } = new List<SpotifySession>();

    public virtual User User { get; set; } = null!;
}
