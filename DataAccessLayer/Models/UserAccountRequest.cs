﻿using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserAccountRequest
{
    public int UserAccountRequestId { get; set; }

    public int UserId { get; set; }

    public Guid RequestCode { get; set; }

    public bool IsRegistrationRequest { get; set; }

    public DateTime GeneratedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
