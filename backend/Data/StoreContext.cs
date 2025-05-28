
using Microsoft.EntityFrameworkCore;
using Streamling.Model.Entities;

namespace Streamling.Data
{
    public class StoreContext(DbContextOptions<StoreContext> options) : DbContext(options)
    {
        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Property> Properties { get; set; }

        public DbSet<Channel> Channels { get; set; }

        public DbSet<Platform> Platforms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Platform>()
            .HasKey(p => p.Name);

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.HasKey(c => new { c.AccountId, c.PlatformName });

                entity.HasOne(c => c.Platform)
                .WithMany(p => p.Channels)
                .HasForeignKey(c => c.PlatformName)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(p => new { p.PlatformPropertyId, p.PlatformName, p.AccountId });

                entity.HasOne(p => p.Platform)
                      .WithMany(p => p.Properties)
                      .HasForeignKey(p => p.PlatformName)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Channel)
                      .WithMany(c => c.Properties)
                      .HasForeignKey(p => new { p.AccountId, p.PlatformName })
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(r => new { r.PlatformName, r.AccountId, r.PlatformPropertyId, r.PlatformReservationId });

                entity.HasOne(r => r.Property)
                      .WithMany(p => p.Reservations)
                      .HasForeignKey(r => new { r.PlatformPropertyId, r.PlatformName, r.AccountId })
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Platform)
                      .WithMany(p => p.Reservations)
                      .HasForeignKey(r => r.PlatformName)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Channel)
                      .WithMany(c => c.Reservations)
                      .HasForeignKey(r => new { r.AccountId, r.PlatformName })
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}