﻿using System.ComponentModel.DataAnnotations;

namespace Vastra.API.Entities
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        public string RoleName { get; set; }
        public DateTime DateAdded { get; private set; }
        public DateTime DateModified { get; private set; }
    }
}
