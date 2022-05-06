using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using moja_druzyna.Models;

namespace moja_druzyna.Data
{
    public partial class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Scout> Scouts { get; set; }
        public virtual DbSet<ScoutTeam> ScoutTeams { get; set; }
        public virtual DbSet<Team> Teams { get; set; }

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

            modelBuilder.Entity<Scout>(entity =>
            {
                entity.HasKey(e => e.Pesel)
                    .HasName("PK__scout__4F16EE7E02333EE9");

                entity.ToTable("scout");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.DateOfBirth)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("date_of_birth");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Nationality)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("nationality");

                entity.Property(e => e.ScoutId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("scout_id");

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

            modelBuilder.Entity<ScoutTeam>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("scout_team");

                entity.Property(e => e.Pesel)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("PESEL");

                entity.Property(e => e.TeamId).HasColumnName("team_id");

                entity.Property(e => e.TeamPosition)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("team_position");

                entity.HasOne(d => d.PeselNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Pesel)
                    .HasConstraintName("FK__scout_tea__team___267ABA7A");

                entity.HasOne(d => d.Team)
                    .WithMany()
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__scout_tea__team___276EDEB3");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("team");

                entity.Property(e => e.TeamId)
                    .ValueGeneratedNever()
                    .HasColumnName("team_id");

                entity.Property(e => e.TeamName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("team_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
