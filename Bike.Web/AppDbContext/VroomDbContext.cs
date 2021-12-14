using Bike.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bike.Web.AppDbContext
{
    public class VroomDbContext : IdentityDbContext
    {
        public VroomDbContext(DbContextOptions<VroomDbContext> options) :
            base(options)
        {
        }

        public DbSet<Make> Makes { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Features> Featuress { get; set; }
        public DbSet<Motorcycle‎> Bikes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        
    }
}
