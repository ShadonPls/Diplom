using DiplomServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<GroupDiscipline> GroupDisciplines => Set<GroupDiscipline>();
        public DbSet<RetakeDirection> RetakeDirections => Set<RetakeDirection>();
        public DbSet<RetakeDirectionStudent> RetakeDirectionStudents => Set<RetakeDirectionStudent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupDiscipline>(entity =>
            {
                entity.ToTable("GroupDisciplines");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => new { x.GroupId, x.DisciplineId, x.AttestTypeId, x.Semester, x.StudyYear }).IsUnique();
            });

            modelBuilder.Entity<RetakeDirection>(entity =>
            {
                entity.ToTable("RetakeDirections");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Status).HasMaxLength(30);
                entity.HasOne(x => x.GroupDiscipline).WithMany(x => x.RetakeDirections).HasForeignKey(x => x.GroupDisciplineId);
            });

            modelBuilder.Entity<RetakeDirectionStudent>(entity =>
            {
                entity.ToTable("RetakeDirectionStudents");
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.RetakeDirection).WithMany(x => x.RetakeDirectionStudents).HasForeignKey(x => x.RetakeDirectionId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(x => new { x.RetakeDirectionId, x.StudentId }).IsUnique();
            });
        }
    }
}