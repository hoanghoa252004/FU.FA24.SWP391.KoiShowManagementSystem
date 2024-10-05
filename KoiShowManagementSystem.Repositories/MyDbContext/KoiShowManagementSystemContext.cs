using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using KoiShowManagementSystem.Entities;

namespace KoiShowManagementSystem.Repositories.MyDbContext;

public partial class KoiShowManagementSystemContext : DbContext
{
    private readonly IConfiguration _configuration;
    public KoiShowManagementSystemContext(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public KoiShowManagementSystemContext(DbContextOptions<KoiShowManagementSystemContext> options, IConfiguration configuration)
        : base(options)
    {
        this._configuration = configuration;
    }

    public virtual DbSet<Criterion> Criteria { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Illustration> Illustrations { get; set; }

    public virtual DbSet<KoiRegistration> KoiRegistrations { get; set; }

    public virtual DbSet<RefereeDetail> RefereeDetails { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Show> Shows { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Variety> Varieties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("cnn"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Criteria__3214EC0765800AAC");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.GroupId).HasColumnName("Group_id");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Group).WithMany(p => p.Criteria)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__Criteria__Group___5EBF139D");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Group__3214EC072D69BC07");

            entity.ToTable("Group");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ShowId).HasColumnName("Show_id");
            entity.Property(e => e.SizeMax)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("Size_max");
            entity.Property(e => e.SizeMin)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("Size_min");

            entity.HasOne(d => d.Show).WithMany(p => p.Groups)
                .HasForeignKey(d => d.ShowId)
                .HasConstraintName("FK__Group__Show_id__4222D4EF");

            entity.HasMany(d => d.Varieties).WithMany(p => p.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "GroupDetail",
                    r => r.HasOne<Variety>().WithMany()
                        .HasForeignKey("VarietyId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GroupDeta__Varie__5BE2A6F2"),
                    l => l.HasOne<Group>().WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GroupDeta__Group__5AEE82B9"),
                    j =>
                    {
                        j.HasKey("GroupId", "VarietyId").HasName("PK__GroupDet__7547B638A9B33778");
                        j.ToTable("GroupDetail");
                        j.IndexerProperty<int>("GroupId").HasColumnName("Group_id");
                        j.IndexerProperty<int>("VarietyId").HasColumnName("Variety_id");
                    });
        });

        modelBuilder.Entity<Illustration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Illustra__3214EC07064AF5B6");

            entity.ToTable("Illustration");

            entity.HasIndex(e => e.KoiId, "UQ_Koi").IsUnique();

            entity.Property(e => e.ImageUrl)
                .HasColumnType("text")
                .HasColumnName("Image_URL");
            entity.Property(e => e.KoiId).HasColumnName("Koi_id");
            entity.Property(e => e.VideoUrl)
                .HasColumnType("text")
                .HasColumnName("Video_URL");

            entity.HasOne(d => d.Koi).WithOne(p => p.Illustration)
                .HasForeignKey<Illustration>(d => d.KoiId)
                .HasConstraintName("FK__Illustrat__Koi_i__534D60F1");
        });

        modelBuilder.Entity<KoiRegistration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KoiRegis__3214EC07C2137D47");

            entity.ToTable("KoiRegistration");

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("Create_date");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.GroupId).HasColumnName("Group_id");
            entity.Property(e => e.IsBestVote).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Size).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TotalScore)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Total_score");
            entity.Property(e => e.UserId).HasColumnName("User_id");
            entity.Property(e => e.VarietyId).HasColumnName("Variety_id");

            entity.HasOne(d => d.Group).WithMany(p => p.KoiRegistrations)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KoiRegist__Group__47DBAE45");

            entity.HasOne(d => d.User).WithMany(p => p.KoiRegistrations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__KoiRegist__User___49C3F6B7");

            entity.HasOne(d => d.Variety).WithMany(p => p.KoiRegistrations)
                .HasForeignKey(d => d.VarietyId)
                .HasConstraintName("FK__KoiRegist__Varie__4AB81AF0");

            entity.HasMany(d => d.Users).WithMany(p => p.Kois)
                .UsingEntity<Dictionary<string, object>>(
                    "Vote",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Vote__User_id__4F7CD00D"),
                    l => l.HasOne<KoiRegistration>().WithMany()
                        .HasForeignKey("KoiId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Vote__Koi_id__4E88ABD4"),
                    j =>
                    {
                        j.HasKey("KoiId", "UserId").HasName("PK__Vote__928E3B0695F4F845");
                        j.ToTable("Vote");
                        j.IndexerProperty<int>("KoiId").HasColumnName("Koi_id");
                        j.IndexerProperty<int>("UserId").HasColumnName("User_id");
                    });
        });

        modelBuilder.Entity<RefereeDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefereeD__3214EC07521349F9");

            entity.ToTable("RefereeDetail");

            entity.HasIndex(e => new { e.UserId, e.ShowId }, "UQ_Referee").IsUnique();

            entity.Property(e => e.ShowId).HasColumnName("Show_id");
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.Show).WithMany(p => p.RefereeDetails)
                .HasForeignKey(d => d.ShowId)
                .HasConstraintName("FK__RefereeDe__Show___5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.RefereeDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RefereeDe__User___571DF1D5");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC07012FB05A");

            entity.ToTable("Role");

            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Score__3214EC0747FD422B");

            entity.ToTable("Score");

            entity.HasIndex(e => new { e.KoiId, e.RefereeDetailId, e.CriteriaId }, "uq_KoiRefereeCriteria").IsUnique();

            entity.Property(e => e.CriteriaId).HasColumnName("Criteria_id");
            entity.Property(e => e.KoiId).HasColumnName("Koi_id");
            entity.Property(e => e.RefereeDetailId).HasColumnName("Referee_detail_id");
            entity.Property(e => e.Score1)
                .HasColumnType("decimal(2, 1)")
                .HasColumnName("Score");

            entity.HasOne(d => d.Criteria).WithMany(p => p.Scores)
                .HasForeignKey(d => d.CriteriaId)
                .HasConstraintName("FK__Score__Criteria___656C112C");

            entity.HasOne(d => d.Koi).WithMany(p => p.Scores)
                .HasForeignKey(d => d.KoiId)
                .HasConstraintName("FK__Score__Koi_id__6383C8BA");

            entity.HasOne(d => d.RefereeDetail).WithMany(p => p.Scores)
                .HasForeignKey(d => d.RefereeDetailId)
                .HasConstraintName("FK__Score__Referee_d__6477ECF3");
        });

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Show__3214EC07099C5E5A");

            entity.ToTable("Show");

            entity.Property(e => e.Banner).HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.RegisterEndDate)
                .HasColumnType("datetime")
                .HasColumnName("Register_end_date");
            entity.Property(e => e.RegisterStartDate)
                .HasColumnType("datetime")
                .HasColumnName("Register_start_date");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07BAC00577");

            entity.ToTable("User");

            entity.HasIndex(e => e.Phone, "UQ__User__5C7E359E5EC1B67A").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D105347747CE78").IsUnique();

            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.RoleId).HasColumnName("Role_id");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__Role_id__3B75D760");
        });

        modelBuilder.Entity<Variety>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Variety__3214EC078B4960BD");

            entity.ToTable("Variety");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Origin).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
