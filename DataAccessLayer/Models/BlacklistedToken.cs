using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class BlacklistedToken
{
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string TokenType { get; set; } = null!;

    public string TokenData { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
