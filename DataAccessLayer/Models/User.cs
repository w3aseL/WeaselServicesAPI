﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models
{
    public partial class User
    {
        public User()
        {
            BlacklistedTokens = new HashSet<BlacklistedToken>();
            SpotifyAccountRequests = new HashSet<SpotifyAccountRequest>();
            SpotifyAccounts = new HashSet<SpotifyAccount>();
            UserAccountRequests = new HashSet<UserAccountRequest>();
        }

        public int UserId { get; set; }
        public Guid Uuid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool VerifiedEmail { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSeed { get; set; }
        public int? RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<BlacklistedToken> BlacklistedTokens { get; set; }
        public virtual ICollection<SpotifyAccountRequest> SpotifyAccountRequests { get; set; }
        public virtual ICollection<SpotifyAccount> SpotifyAccounts { get; set; }
        public virtual ICollection<UserAccountRequest> UserAccountRequests { get; set; }
    }
}