using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class AuthenticationMethod
{
    public int AuthenticationMethodId { get; set; }

    public int UserId { get; set; }

    public string? SecretKey { get; set; }

    public string? PhoneNumber { get; set; }

    public int AuthenticationTypeId { get; set; }

    public int? PriorityOrder { get; set; }

    public virtual User User { get; set; } = null!;
}
