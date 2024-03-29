﻿using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class SpotifyAccountRequest
{
    public int SpotifyAccountRequestId { get; set; }

    public int UserId { get; set; }

    public Guid AuthorizationCode { get; set; }

    public DateTime ExpirationDate { get; set; }

    public string? OriginalUrl { get; set; }

    public virtual User User { get; set; } = null!;
}
