﻿using System.ComponentModel.DataAnnotations.Schema;
using Vastra.API.Entities;

namespace Vastra.API.Models
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string? LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string? EmailId { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        
    }
}
