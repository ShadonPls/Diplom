using Microsoft.EntityFrameworkCore;

namespace DiplomServer.Infrastructure.Data
{
    
    public class ScheduleDbContext : DbContext
    {
        public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : base(options) { }

        public DbSet<ScheduleGroup> Groups => Set<ScheduleGroup>();
        public DbSet<ScheduleDiscipline> Disciplines => Set<ScheduleDiscipline>();
        public DbSet<ScheduleTeacher> Teachers => Set<ScheduleTeacher>();
        public DbSet<ScheduleAttestation> Attestations => Set<ScheduleAttestation>();
        public DbSet<ScheduleTeacherDiscipline> TeachersDisciplines => Set<ScheduleTeacherDiscipline>();
        public DbSet<ScheduleUser> Users => Set<ScheduleUser>();
        public DbSet<SchedulePlanDiscipline> PlansDisciplines => Set<SchedulePlanDiscipline>();
        public DbSet<ScheduleAcademicPlans> AcademicPlans => Set<ScheduleAcademicPlans>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging().EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SchedulePlanDiscipline>().ToTable("plans_disciplines").HasKey(x => x.Id);
            modelBuilder.Entity<ScheduleAcademicPlans>().ToTable("academic_plans").HasKey(x => x.Id);
            modelBuilder.Entity<ScheduleUser>().ToTable("users").HasKey(x => x.Id);
            modelBuilder.Entity<ScheduleGroup>().ToTable("groups").HasKey(x => x.Id);
            modelBuilder.Entity<ScheduleDiscipline>().ToTable("disciplines").HasKey(x => x.Id);
            modelBuilder.Entity<ScheduleTeacher>().ToTable("teachers").HasKey(x => x.Id);
            modelBuilder.Entity<ScheduleAttestation>().ToTable("attestations").HasKey(x => x.Id);
            modelBuilder.Entity<ScheduleTeacherDiscipline>().ToTable("teachers_disciplines").HasKey(x => x.Id);
        }
    }
    public class ScheduleUser
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";
        public uint IdUser { get; set; }
    }
    public class ScheduleGroup
    {
        public uint Id { get; set; }
        public uint? id_academic_plans { get; set; }
        public int Year { get; set; }
        public int Number { get; set; }
        public int Delete { get; set; } 
    }
    public class ScheduleAcademicPlans
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public int Delete { get; set; }
    }
    public class ScheduleDiscipline
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Delete { get; set; } 
    }
    public class SchedulePlanDiscipline
    {
        public int Id { get; set; }
        public int id_discipline { get; set; } 
        public uint id_academic_plan { get; set; }
        public int Delete { get; set; }

        public ScheduleDiscipline Discipline { get; set; } = null!;
    }
    public class ScheduleTeacher
    {
        public uint Id { get; set; }
        public string first_name { get; set; } = "";
        public string last_name { get; set; } = "";
        public string middle_name { get; set; } = "";
        public int Delete { get; set; }
    }

    public class ScheduleAttestation
    {
        public uint Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class ScheduleTeacherDiscipline
    {
        public int Id { get; set; }
        public int id_plan_discipline { get; set; }
        public int id_teacher { get; set; }        
        public int id_group { get; set; }
        public int Delete { get; set; }

        public SchedulePlanDiscipline PlanDiscipline { get; set; } = null!;
    }
}