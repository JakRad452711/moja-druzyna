using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using moja_druzyna.Models;

namespace moja_druzyna.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<Address> Adresses { get; set; }
        public virtual DbSet<Agreement> Agreements { get; set; }
        public virtual DbSet<AttendanceList> AttendanceLists { get; set; }
        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<DutyHistory> DutyHistories { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventTeam> EventTeams { get; set; }
        public virtual DbSet<Host> Hosts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderInfo> OrderInfos { get; set; }
        public virtual DbSet<Parent> Parents { get; set; }
        public virtual DbSet<Rank> Ranks { get; set; }
        public virtual DbSet<Scout> Scouts { get; set; }
        public virtual DbSet<ScoutAchievement> ScoutAchievements { get; set; }
        public virtual DbSet<ScoutAgreement> ScoutAgreements { get; set; }
        public virtual DbSet<ScoutCollection> ScoutCollections { get; set; }
        public virtual DbSet<ScoutCourse> ScoutCourses { get; set; }
        public virtual DbSet<ScoutEvent> ScoutEvents { get; set; }
        public virtual DbSet<ScoutRank> ScoutRanks { get; set; }
        public virtual DbSet<ScoutHost> ScoutHost { get; set; }
        public virtual DbSet<ScoutTeam> ScoutTeam { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TrainingCourse> TrainingCourses { get; set; }
        public virtual DbSet <Points> Points { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MojaDruzyna;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Achievement>(entity =>
            {
         
                entity.ToTable("achievement");


            });

            modelBuilder.Entity<Address>(entity =>
            {


                entity.ToTable("address");

                entity.HasKey(e => new { e.ParentPesel, e.ScoutPeselScout })
                .HasName("PK_adresss_quicksolve");



            });

            modelBuilder.Entity<Agreement>(entity =>
            {
       

                entity.ToTable("agreement");

            });

            modelBuilder.Entity<AttendanceList>(entity =>
            {
               
                entity.ToTable("attendance_list");


            });

            modelBuilder.Entity<Collection>(entity =>
            {
         

                entity.ToTable("collection");


            });

            modelBuilder.Entity<DutyHistory>(entity =>
            {
                entity.HasKey(e => new { e.ScoutPeselScout, e.Team, e.Host, e.DateStart, e.Banner });

                entity.ToTable("duty_history");


            });

            modelBuilder.Entity<Event>(entity =>
            {
               

                entity.ToTable("event");


            });

            modelBuilder.Entity<EventTeam>(entity =>
            {
                entity.HasKey(e => new { e.EventIdEvent, e.TeamIdTeam })
                    .HasName("PK__event_te__FD53622128F556BD");

                entity.ToTable("event_team");


            });

            modelBuilder.Entity<Host>(entity =>
            {
               

                entity.ToTable("host");


            });

            modelBuilder.Entity<Parent>(entity =>
            {

                entity.ToTable("parent");


            });
            modelBuilder.Entity<Points>(entity =>
            {

                entity.ToTable("Points");

                entity.HasKey(e => new { e.DateAcquirement,e.OrderId,e.ScoutPeselScout })
               .HasName("PK__points__FD53622128F556BD");


            });

            modelBuilder.Entity<Rank>(entity =>
            {
               

                entity.ToTable("rank");

            });

            modelBuilder.Entity<Scout>(entity =>
            {
                

                entity.ToTable("scout");

            });

            modelBuilder.Entity<ScoutAchievement>(entity =>
            {
                entity.HasKey(e => new { e.ScoutPeselScout, e.AchievementIdAchievement })
                    .HasName("PK__scout_achievement__14C192AA7E8B153B");

                entity.ToTable("scout_achievement");

            });

            modelBuilder.Entity<ScoutAgreement>(entity =>
            {
                entity.HasKey(e => new { e.ScoutPeselScout, e.AgreementIdAgreement })
                    .HasName("PK__scout_ag__14C192AA7E8B153B");

                entity.ToTable("scout_agreement");

            });

            modelBuilder.Entity<ScoutCollection>(entity =>
            {
                entity.HasKey(e => new { e.ScoutPeselScout, e.CollectionIdCollection })
                    .HasName("PK__scout_co__EDEC6AB95EAAD616");

                entity.ToTable("scout_collection");


            });

            modelBuilder.Entity<ScoutCourse>(entity =>
            {
                entity.HasKey(e => new { e.ScoutPeselScout, e.TrainingCourseIdCourse })
                    .HasName("PK__scout_co__F0AEC3E099E17797");

                entity.ToTable("scout_course");



                modelBuilder.Entity<ScoutEvent>(entity =>
                {
                    entity.HasKey(e => new { e.ScoutPeselScout, e.EventIdEvent })
                        .HasName("PK__scout_ev__A6050A5886C962DC");

                    entity.ToTable("scout_event");


                });

                modelBuilder.Entity<ScoutRank>(entity =>
                {
                    entity.HasKey(e => new { e.ScoutPeselScout, e.RankName })
                        .HasName("PK__scout_ra__F838FC8F4D9C216B");

                    entity.ToTable("scout_rank");


                });

                modelBuilder.Entity<ScoutHost>(entity =>
                {
                    entity.HasKey(e => new { e.ScoutPeselScout, e.HostIdHost })
                        .HasName("PK__scout_host__84F25C26A6B4D35B");

                    entity.ToTable("scout_host");


                });
                modelBuilder.Entity<ScoutTeam>(entity =>
                {
                    entity.HasKey(e => new { e.ScoutPeselScout, e.TeamIdTeam })
                        .HasName("PK__scout_team__84F25C26A6B4D35B");

                    entity.ToTable("scout_team");


                });

                modelBuilder.Entity<Team>(entity =>
                {
                    

                });

                modelBuilder.Entity<TrainingCourse>(entity =>
                {
                   



                });
            });
        }
    }
}
