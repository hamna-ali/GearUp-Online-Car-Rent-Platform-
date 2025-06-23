using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GearUp.Models;

namespace GearUp.Data
{
    // Inherit from IdentityDbContext to integrate ASP.NET Identity
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet for Vehicle entity
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        //public IEnumerable<object> CartItems { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly map the Vehicle entity to the 'Vehicle' table (singular)
           modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
        }
    }
}
