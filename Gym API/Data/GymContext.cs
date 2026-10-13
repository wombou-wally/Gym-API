using Gym_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym_API.Data
{
    public class GymContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<MembershipPlan> Plans { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<User> Users { get; set; }

        public GymContext(DbContextOptions<GymContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}