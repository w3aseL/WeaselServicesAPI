﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models
{
    public partial class Permission
    {
        public Permission()
        {
            Roles = new HashSet<Role>();
        }

        public int PermissionId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}