using DiplomServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<Discipline> Disciplines => Set<Discipline>();
        public DbSet<AttestType> AttestTypes => Set<AttestType>();
        public DbSet<GroupDiscipline> GroupDisciplines => Set<GroupDiscipline>();
        public DbSet<RetakeDirection> RetakeDirections => Set<RetakeDirection>();
        public DbSet<RetakeDirectionStudent> RetakeDirectionStudents => Set<RetakeDirectionStudent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUser(modelBuilder);
            ConfigureTeacher(modelBuilder);
            ConfigureGroup(modelBuilder);
            ConfigureStudent(modelBuilder);
            ConfigureDiscipline(modelBuilder);
            ConfigureAttestType(modelBuilder);
            ConfigureGroupDiscipline(modelBuilder);
            ConfigureRetakeDirection(modelBuilder);
            ConfigureRetakeDirectionStudent(modelBuilder);
        }

        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(x => x.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(x => x.Role)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.HasOne(x => x.Teacher)
                    .WithMany(t => t.Users)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(x => x.RetakeDirectionsCreated)
                    .WithOne(rd => rd.CreatedBy)
                    .HasForeignKey(rd => rd.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureTeacher(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teachers");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Surname)
                    .HasMaxLength(100);
            });
        }

        private static void ConfigureGroup(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("Groups");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });
        }

        private static void ConfigureStudent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Surname)
                    .HasMaxLength(100);

                entity.Property(x => x.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.HasOne(x => x.Group)
                    .WithMany(g => g.Students)
                    .HasForeignKey(x => x.GroupId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureDiscipline(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Discipline>(entity =>
            {
                entity.ToTable("Disciplines");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasIndex(x => x.Name);
            });
        }

        private static void ConfigureAttestType(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttestType>(entity =>
            {
                entity.ToTable("AttestTypes");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });
        }

        private static void ConfigureGroupDiscipline(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupDiscipline>(entity =>
            {
                entity.ToTable("GroupDisciplines");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Semester)
                    .IsRequired();

                entity.Property(x => x.StudyYear)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(x => x.Group)
                    .WithMany(g => g.GroupDisciplines)
                    .HasForeignKey(x => x.GroupId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Discipline)
                    .WithMany(d => d.GroupDisciplines)
                    .HasForeignKey(x => x.DisciplineId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Teacher)
                    .WithMany(t => t.GroupDisciplines)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.AttestType)
                    .WithMany(a => a.GroupDisciplines)
                    .HasForeignKey(x => x.AttestTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.GroupId,
                    x.DisciplineId,
                    x.TeacherId,
                    x.AttestTypeId,
                    x.Semester,
                    x.StudyYear
                }).IsUnique();
            });
        }

        private static void ConfigureRetakeDirection(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RetakeDirection>(entity =>
            {
                entity.ToTable("RetakeDirections");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.Number)
                    .HasMaxLength(100);

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.Property(x => x.RetakeDate)
                    .IsRequired();

                entity.HasOne(x => x.GroupDiscipline)
                    .WithMany(gd => gd.RetakeDirections)
                    .HasForeignKey(x => x.GroupDisciplineId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.CreatedBy)
                    .WithMany(u => u.RetakeDirectionsCreated)
                    .HasForeignKey(x => x.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.Status);
                entity.HasIndex(x => x.CreatedAt);
            });
        }

        private static void ConfigureRetakeDirectionStudent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RetakeDirectionStudent>(entity =>
            {
                entity.ToTable("RetakeDirectionStudents");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.RetakeGradeValue)
                    .IsRequired();

                entity.Property(x => x.RetakeIsPassed)
                    .IsRequired();

                entity.Property(x => x.RetakeGradeDate)
                    .IsRequired();

                entity.HasOne(x => x.RetakeDirection)
                    .WithMany(rd => rd.RetakeDirectionStudents)
                    .HasForeignKey(x => x.RetakeDirectionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Student)
                    .WithMany(s => s.RetakeDirectionStudents)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.RetakeDirectionId, x.StudentId })
                    .IsUnique();
            });
        }
    }
}