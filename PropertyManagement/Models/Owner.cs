﻿using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Models
{
    public class Owner
    {
        [Key]
        public int OwnerID { get; set; }
        public IEnumerable<Property> Properties { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}