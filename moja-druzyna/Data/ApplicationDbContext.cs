using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using moja_druzyna.Models;

namespace moja_druzyna.Data
{
    public class ApplicationDbContext : IdentityDbContext<Scout>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /*public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<Adress> Adresses { get; set; } 
        public virtual DbSet<Agreement> Agreements { get; set; }
        public virtual DbSet<AttendanceList> AttendanceLists { get; set; }
        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<DutyHistory> DutyHistories{ get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventTeam> EventTeams { get; set; }
        public virtual DbSet<Host> Hosts { get; set; }
        public virtual DbSet<Parent> Parents { get; set; }
        public virtual DbSet<Rank> Ranks { get; set; }*/
        public virtual DbSet<Scout> Scouts { get; set; }
        /*public virtual DbSet<ScoutAchievement> ScoutAchievements { get; set; }
        public virtual DbSet<ScoutAgreement> ScoutAgreements{ get; set; }
        public virtual DbSet<ScoutCollection> ScoutCollections { get; set; }
        public virtual DbSet<ScoutCourse> ScoutCourses { get; set; }
        public virtual DbSet<ScoutEvent> ScoutEvents { get; set; }
        public virtual DbSet<ScoutRank> ScoutRanks { get; set; }
        public virtual DbSet<ScoutTeam> ScoutTeams { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TrainingCourse> TrainingCourses { get; set; }*/

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

            /*modelBuilder.Entity<Achievement>(entity =>
            {
                entity.HasKey(e => e.IdAchievement)
                    .HasName("PK__achievem__719AF69DD4D4300D");

                entity.ToTable("achievement");

                entity.Property(e => e.IdAchievement)
                    .ValueGeneratedNever()
                    .HasColumnName("id_achievement");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<Adress>(entity =>
            {
                entity.HasKey(e => new { e.PeselScout, e.PeselParent })
                    .HasName("PK__adress__A4FDAC3BFD1F6F63");

                entity.ToTable("adress");

                entity.Property(e => e.PeselScout)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL_scout");

                entity.Property(e => e.PeselParent)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL_parent");

                entity.Property(e => e.AddresZam)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("addres_zam");

                entity.Property(e => e.AdressKor)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("adress_kor");

                entity.Property(e => e.CityKor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("city_kor");

                entity.Property(e => e.CityZam)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("city_zam");

                entity.Property(e => e.CountryKor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("country_kor");

                entity.Property(e => e.CountryZam)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("country_zam");

                entity.Property(e => e.HouseKor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("house_kor");

                entity.Property(e => e.HouseZam)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("house_zam");

                entity.Property(e => e.StreatKor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("streat_kor");

                entity.Property(e => e.StreatZam)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("streat_zam");

                entity.Property(e => e.ZipKor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("zip_kor");

                entity.Property(e => e.ZipZam)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("zip_zam");

                entity.HasOne(d => d.PeselParentNavigation)
                    .WithMany(p => p.Adresses)
                    .HasForeignKey(d => d.PeselParent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__adress__PESEL_pa__4316F928");

                entity.HasOne(d => d.PeselScoutNavigation)
                    .WithMany(p => p.Adresses)
                    .HasForeignKey(d => d.PeselScout)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__adress__PESEL_sc__4222D4EF");
            });

            modelBuilder.Entity<Agreement>(entity =>
            {
                entity.HasKey(e => e.IdAgreement)
                    .HasName("PK__agreemen__BD77CD4459B3997E");

                entity.ToTable("agreement");

                entity.Property(e => e.IdAgreement)
                    .ValueGeneratedNever()
                    .HasColumnName("id_agreement");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<AttendanceList>(entity =>
            {
                entity.HasKey(e => e.IdList)
                    .HasName("PK__attendan__998043075E6BBA01");

                entity.ToTable("attendance_list");

                entity.Property(e => e.IdList)
                    .ValueGeneratedNever()
                    .HasColumnName("id_list");

                entity.Property(e => e.DateOfList)
                    .HasColumnType("date")
                    .HasColumnName("date_of_list");

                entity.Property(e => e.IdEvent).HasColumnName("id_event");

                entity.Property(e => e.IdHost).HasColumnName("id_host");

                entity.Property(e => e.IdTeam).HasColumnName("id_team");

                entity.HasOne(d => d.IdEventNavigation)
                    .WithMany(p => p.AttendanceLists)
                    .HasForeignKey(d => d.IdEvent)
                    .HasConstraintName("FK__attendanc__id_ev__3A81B327");

                entity.HasOne(d => d.IdHostNavigation)
                    .WithMany(p => p.AttendanceLists)
                    .HasForeignKey(d => d.IdHost)
                    .HasConstraintName("FK__attendanc__id_ho__3B75D760");

                entity.HasOne(d => d.IdTeamNavigation)
                    .WithMany(p => p.AttendanceLists)
                    .HasForeignKey(d => d.IdTeam)
                    .HasConstraintName("FK__attendanc__id_te__3C69FB99");
            });

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.HasKey(e => e.IdCollection)
                    .HasName("PK__collecti__2FA84C7B2F21E45D");

                entity.ToTable("collection");

                entity.Property(e => e.IdCollection)
                    .ValueGeneratedNever()
                    .HasColumnName("id_collection");

                entity.Property(e => e.IdEvent).HasColumnName("id_event");

                entity.Property(e => e.Quarter).HasColumnName("quarter");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.HasOne(d => d.IdEventNavigation)
                    .WithMany(p => p.Collections)
                    .HasForeignKey(d => d.IdEvent)
                    .HasConstraintName("FK__collectio__id_ev__3F466844");
            });

            modelBuilder.Entity<DutyHistory>(entity =>
            {
                entity.HasKey(e => e.Pesel)
                    .HasName("PK__duty_his__4F16EE7EF330C483");

                entity.ToTable("duty_history");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.Banner)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("banner");

                entity.Property(e => e.DateEnd)
                    .HasColumnType("date")
                    .HasColumnName("date_end");

                entity.Property(e => e.DateStart)
                    .HasColumnType("date")
                    .HasColumnName("date_start");

                entity.Property(e => e.Detachment)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("detachment");

                entity.Property(e => e.Host)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("host");

                entity.Property(e => e.Team)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("team");

                entity.HasOne(d => d.PeselNavigation)
                    .WithOne(p => p.DutyHistory)
                    .HasForeignKey<DutyHistory>(d => d.Pesel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__duty_hist__PESEL__45F365D3");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.IdEvent)
                    .HasName("PK__event__913E426F5E65A2AF");

                entity.ToTable("event");

                entity.Property(e => e.IdEvent)
                    .ValueGeneratedNever()
                    .HasColumnName("id_event");

                entity.Property(e => e.DateStartDateNotNullDateEnd)
                    .HasColumnType("date")
                    .HasColumnName("date_start DATE NOT NULL,\r\n  [date_end");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.HasCost)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("has_cost");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<EventTeam>(entity =>
            {
                entity.HasKey(e => new { e.IdEvent, e.IdTeam })
                    .HasName("PK__event_te__FD53622128F556BD");

                entity.ToTable("event_team");

                entity.Property(e => e.IdEvent).HasColumnName("id_event");

                entity.Property(e => e.IdTeam).HasColumnName("id_team");

                entity.HasOne(d => d.IdEventNavigation)
                    .WithMany(p => p.EventTeams)
                    .HasForeignKey(d => d.IdEvent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__event_tea__id_ev__30F848ED");

                entity.HasOne(d => d.IdTeamNavigation)
                    .WithMany(p => p.EventTeams)
                    .HasForeignKey(d => d.IdTeam)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__event_tea__id_te__31EC6D26");
            });

            modelBuilder.Entity<Host>(entity =>
            {
                entity.HasKey(e => e.IdHost)
                    .HasName("PK__host__BE4B258DF15A7A9E");

                entity.ToTable("host");

                entity.Property(e => e.IdHost)
                    .ValueGeneratedNever()
                    .HasColumnName("id_host");

                entity.Property(e => e.IdTeam).HasColumnName("id_team");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdTeamNavigation)
                    .WithMany(p => p.Hosts)
                    .HasForeignKey(d => d.IdTeam)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__host__id_team__37A5467C");
            });

            modelBuilder.Entity<Parent>(entity =>
            {
                entity.HasKey(e => e.Pesel)
                    .HasName("PK__parent__4F16EE7EEBB3FF89");

                entity.ToTable("parent");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("password")
                    .IsFixedLength(true);

                entity.Property(e => e.SecondName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("second_name");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("surname");
            });

            modelBuilder.Entity<Rank>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__rank__72E12F1ABDB20DA1");

                entity.ToTable("rank");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });*/

            modelBuilder.Entity<Scout>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK__scout__8D9BB9CB49FD65FE");

                entity.ToTable("scout");

                entity.Property(e => e.PeselScout)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL_scout");

                entity.Property(e => e.CrossNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cross_number");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("date_of_birth");

                entity.Property(e => e.DateOfEntry)
                    .HasColumnType("date")
                    .HasColumnName("date_of_entry");

                entity.Property(e => e.DateOfLeaving)
                    .HasColumnType("date")
                    .HasColumnName("date_of_leaving");

                entity.Property(e => e.MembershipNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("membership_number");

                entity.Property(e => e.Nationality)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nationality");

                entity.Property(e => e.Ns)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("NS");

                entity.Property(e => e.PeselParent)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL_parent");

                entity.Property(e => e.SecondName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("second_name");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("surname");

                /*entity.HasOne(d => d.PeselParentNavigation)
                    .WithMany(p => p.Scouts)
                    .HasForeignKey(d => d.PeselParent)
                    .HasConstraintName("FK__scout__PESEL_par__34C8D9D1");*/
            });

            /*modelBuilder.Entity<ScoutAchievement>(entity =>
            {
                entity.HasKey(e => e.Pesel)
                    .HasName("PK__scout_ac__4F16EE7E0DF9BDD4");

                entity.ToTable("scout_achievement");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.IdAchievement).HasColumnName("id_achievement");

                entity.HasOne(d => d.IdAchievementNavigation)
                    .WithMany(p => p.ScoutAchievements)
                    .HasForeignKey(d => d.IdAchievement)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_ach__id_ac__4D94879B");

                entity.HasOne(d => d.PeselNavigation)
                    .WithOne(p => p.ScoutAchievement)
                    .HasForeignKey<ScoutAchievement>(d => d.Pesel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_ach__PESEL__4CA06362");
            });

            modelBuilder.Entity<ScoutAgreement>(entity =>
            {
                entity.HasKey(e => new { e.Pesel, e.IdAgreement })
                    .HasName("PK__scout_ag__14C192AA7E8B153B");

                entity.ToTable("scout_agreement");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.IdAgreement).HasColumnName("id_agreement");

                entity.Property(e => e.DataCancel)
                    .HasColumnType("date")
                    .HasColumnName("data_cancel");

                entity.Property(e => e.DateSign)
                    .HasColumnType("date")
                    .HasColumnName("date_sign");

                entity.HasOne(d => d.IdAgreementNavigation)
                    .WithMany(p => p.ScoutAgreements)
                    .HasForeignKey(d => d.IdAgreement)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_agr__id_ag__49C3F6B7");

                entity.HasOne(d => d.PeselNavigation)
                    .WithMany(p => p.ScoutAgreements)
                    .HasForeignKey(d => d.Pesel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_agr__PESEL__48CFD27E");
            });

            modelBuilder.Entity<ScoutCollection>(entity =>
            {
                entity.HasKey(e => new { e.Pesel, e.IdCollection })
                    .HasName("PK__scout_co__EDEC6AB95EAAD616");

                entity.ToTable("scout_collection");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.IdCollection).HasColumnName("id_collection");

                entity.Property(e => e.Advance)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("advance");

                entity.Property(e => e.Ammount).HasColumnName("ammount");

                entity.Property(e => e.DateAcquirement)
                    .HasColumnType("date")
                    .HasColumnName("date_acquirement");

                entity.HasOne(d => d.IdCollectionNavigation)
                    .WithMany(p => p.ScoutCollections)
                    .HasForeignKey(d => d.IdCollection)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_col__id_co__59063A47");

                entity.HasOne(d => d.PeselNavigation)
                    .WithMany(p => p.ScoutCollections)
                    .HasForeignKey(d => d.Pesel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_col__PESEL__5812160E");
            });

            modelBuilder.Entity<ScoutCourse>(entity =>
            {
                entity.HasKey(e => new { e.Pesel, e.IdCourse })
                    .HasName("PK__scout_co__F0AEC3E099E17797");

                entity.ToTable("scout_course");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.IdCourse).HasColumnName("id_course");

                entity.Property(e => e.DateAcquirement)
                    .HasColumnType("date")
                    .HasColumnName("date_acquirement");

                entity.HasOne(d => d.IdCourseNavigation)
                    .WithMany(p => p.ScoutCourses)
                    .HasForeignKey(d => d.IdCourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_cou__id_co__5165187F");

                entity.HasOne(d => d.PeselNavigation)
                    .WithMany(p => p.ScoutCourses)
                    .HasForeignKey(d => d.Pesel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_cou__PESEL__5070F446");
            });

            modelBuilder.Entity<ScoutEvent>(entity =>
            {
                entity.HasKey(e => new { e.Pesel, e.IdEvent })
                    .HasName("PK__scout_ev__A6050A5886C962DC");

                entity.ToTable("scout_event");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.IdEvent).HasColumnName("id_event");

                entity.HasOne(d => d.IdEventNavigation)
                    .WithMany(p => p.ScoutEvents)
                    .HasForeignKey(d => d.IdEvent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_eve__id_ev__5CD6CB2B");

                entity.HasOne(d => d.PeselNavigation)
                    .WithMany(p => p.ScoutEvents)
                    .HasForeignKey(d => d.Pesel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_eve__PESEL__5BE2A6F2");
            });

            modelBuilder.Entity<ScoutRank>(entity =>
            {
                entity.HasKey(e => new { e.Pesel, e.Name })
                    .HasName("PK__scout_ra__F838FC8F4D9C216B");

                entity.ToTable("scout_rank");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.DateAcquirement)
                    .HasColumnType("date")
                    .HasColumnName("date_acquirement");

                entity.HasOne(d => d.NameNavigation)
                    .WithMany(p => p.ScoutRanks)
                    .HasForeignKey(d => d.Name)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_rank__name__60A75C0F");

                entity.HasOne(d => d.PeselNavigation)
                    .WithMany(p => p.ScoutRanks)
                    .HasForeignKey(d => d.Pesel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_ran__PESEL__5FB337D6");
            });

            modelBuilder.Entity<ScoutTeam>(entity =>
            {
                entity.HasKey(e => new { e.Pesel, e.IdHost })
                    .HasName("PK__scout_te__84F25C26A6B4D35B");

                entity.ToTable("scout_team");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.IdHost).HasColumnName("id_host");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("role");

                entity.HasOne(d => d.IdHostNavigation)
                    .WithMany(p => p.ScoutTeams)
                    .HasForeignKey(d => d.IdHost)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_tea__id_ho__5535A963");

                entity.HasOne(d => d.PeselNavigation)
                    .WithMany(p => p.ScoutTeams)
                    .HasForeignKey(d => d.Pesel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__scout_tea__PESEL__5441852A");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.IdTeam)
                    .HasName("PK__team__C6D204E72A154897");

                entity.ToTable("team");

                entity.Property(e => e.IdTeam)
                    .ValueGeneratedNever()
                    .HasColumnName("id_team");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<TrainingCourse>(entity =>
            {
                entity.HasKey(e => e.IdCourse)
                    .HasName("PK__training__FB82D9EA6AABE2C5");

                entity.ToTable("training_course");

                entity.Property(e => e.IdCourse)
                    .ValueGeneratedNever()
                    .HasColumnName("id_course");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });*/
        }
    }
}
