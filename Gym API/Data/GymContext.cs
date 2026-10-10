using Gym_API.Data;
using Gym_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gym_API.Data
{
    public class GymContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<MembershipPlan> Plans { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
        public GymContext(DbContextOptions<GymContext> options) : base(options)
        {
            
        }
    }
}
