using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Models;

namespace PropertyManagement.Data
{
    public class PropertyManagementContext : DbContext
    {
        public PropertyManagementContext (DbContextOptions<PropertyManagementContext> options)
            : base(options)
        {
        }

        public DbSet<PropertyManagement.Models.Property> Property { get; set; } = default!;

        public DbSet<PropertyManagement.Models.Owner>? Owner { get; set; }
    }
}
