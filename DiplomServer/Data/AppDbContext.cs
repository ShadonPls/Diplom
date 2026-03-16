using DiplomServer.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AttestType> AttestTypes => Set<AttestType>();
        public DbSet<Discipline> Disciplines => Set<Discipline>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<GroupDiscipline> GroupDisciplines => Set<GroupDiscipline>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<RetakeDirection> RetakeDirections => Set<RetakeDirection>();
        public DbSet<RetakeDirectionStudent> RetakeDirectionStudents => Set<RetakeDirectionStudent>();
        public DbSet<User> Users => Set<User>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId);

            modelBuilder.Entity<GroupDiscipline>()
                .HasOne(gd => gd.Group)
                .WithMany(g => g.GroupDisciplines)
                .HasForeignKey(gd => gd.GroupId);

            modelBuilder.Entity<GroupDiscipline>()
                .HasOne(gd => gd.Discipline)
                .WithMany(d => d.GroupDisciplines)
                .HasForeignKey(gd => gd.DisciplineId);

            modelBuilder.Entity<GroupDiscipline>()
                .HasOne(gd => gd.Teacher)
                .WithMany(t => t.GroupDisciplines)
                .HasForeignKey(gd => gd.TeacherId);

            modelBuilder.Entity<GroupDiscipline>()
                .HasOne(gd => gd.AttestType)
                .WithMany(a => a.GroupDisciplines)
                .HasForeignKey(gd => gd.AttestTypeId);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.GroupDiscipline)
                .WithMany(gd => gd.Grades)
                .HasForeignKey(g => g.GroupDisciplineId);

            modelBuilder.Entity<RetakeDirection>()
                .HasOne(rd => rd.GroupDiscipline)
                .WithMany(gd => gd.RetakeDirections)
                .HasForeignKey(rd => rd.GroupDisciplineId);

            modelBuilder.Entity<RetakeDirection>()
                .HasOne(rd => rd.CreatedBy)
                .WithMany(u => u.RetakeDirectionsCreated)
                .HasForeignKey(rd => rd.CreatedById);

            modelBuilder.Entity<RetakeDirectionStudent>()
                .HasOne(rds => rds.RetakeDirection)
                .WithMany(rd => rd.RetakeDirectionStudents)
                .HasForeignKey(rds => rds.RetakeDirectionId);

            modelBuilder.Entity<RetakeDirectionStudent>()
                .HasOne(rds => rds.Student)
                .WithMany(s => s.RetakeDirectionStudents)
                .HasForeignKey(rds => rds.StudentId);
        }
    }
}
