using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public int UserId { get; set; }

    public Guid Uuid { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public byte[]? Password { get; set; }

    public byte[]? PasswordSeed { get; set; }

    public virtual ICollection<BlacklistedToken> BlacklistedTokens { get; } = new List<BlacklistedToken>();

    public virtual ICollection<SpotifyAccountRequest> SpotifyAccountRequests { get; } = new List<SpotifyAccountRequest>();

    public virtual ICollection<SpotifyAccount> SpotifyAccounts { get; } = new List<SpotifyAccount>();

    public virtual ICollection<UserAccountRequest> UserAccountRequests { get; } = new List<UserAccountRequest>();
}
