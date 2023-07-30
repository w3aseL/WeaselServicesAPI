using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public int UserId { get; set; }

    public Guid Uuid { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public bool VerifiedEmail { get; set; }

    public byte[]? Password { get; set; }

    public byte[]? PasswordSeed { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<BlacklistedToken> BlacklistedTokens { get; set; } = new List<BlacklistedToken>();

    public virtual ICollection<BlogAuthor> BlogAuthors { get; set; } = new List<BlogAuthor>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<SpotifyAccountRequest> SpotifyAccountRequests { get; set; } = new List<SpotifyAccountRequest>();

    public virtual ICollection<SpotifyAccount> SpotifyAccounts { get; set; } = new List<SpotifyAccount>();

    public virtual ICollection<UserAccountRequest> UserAccountRequests { get; set; } = new List<UserAccountRequest>();
}
