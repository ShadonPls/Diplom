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


        public DbSet<Orders> Orders => Set<Orders>();
        public DbSet<ReceptionTeacher> ReceptionTeachers => Set<ReceptionTeacher>();
        public DbSet<ReceptionCommission> ReceptionCommissions => Set<ReceptionCommission>();

        public DbSet<AcademicDebts> AcademicDebts => Set<AcademicDebts>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupDiscipline>(entity =>
            {
                entity.ToTable("GroupDisciplines");
                entity.HasKey(x => x.Id);
            });
            modelBuilder.Entity<AcademicDebts>(entity =>
            {
                entity.ToTable("AcademicDebts");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<RetakeDirection>(entity =>
            {
                entity.ToTable("RetakeDirections");
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.GroupDiscipline).WithMany(x => x.RetakeDirections).HasForeignKey(x => x.GroupDisciplineId);
            });

            modelBuilder.Entity<RetakeDirectionStudent>(entity =>
            {
                entity.ToTable("RetakeDirectionStudents");
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.RetakeDirection).WithMany(x => x.RetakeDirectionStudents).HasForeignKey(x => x.RetakeDirectionId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(x => new { x.RetakeDirectionId, x.StudentId }).IsUnique();
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Number).IsRequired().HasMaxLength(50);
                entity.Property(x => x.DateCreate).HasColumnType("date");
            });

            modelBuilder.Entity<ReceptionTeacher>(entity =>
            {
                entity.ToTable("ReceptionTeacher");
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Order)
                      .WithMany(x => x.ReceptionTeachers)
                      .HasForeignKey(x => x.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ReceptionCommission>(entity =>
            {
                entity.ToTable("ReceptionCommission");
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Order)
                      .WithMany(x => x.ReceptionCommissions)
                      .HasForeignKey(x => x.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}