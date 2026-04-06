using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Infrastructure.Data
{
    public class VrStudent
    {
        public uint Id { get; set; }
        public string Firstname { get; set; } = "";
        public string Lastname { get; set; } = "";
        public string Surname { get; set; } = "";
        public string Group { get; set; } = "";
        public string? Deduction { get; set; }
    }

    public class VrDbContext : DbContext
    {
        public VrDbContext(DbContextOptions<VrDbContext> options) : base(options) { }

        public DbSet<VrStudent> Students => Set<VrStudent>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging().EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VrStudent>()
                .ToTable("students")
                .HasKey(x => x.Id);
        }
    }
}