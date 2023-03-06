using System;
using System.Collections.Generic;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer;

public partial class ServicesAPIContext : DbContext
{
    public ServicesAPIContext()
    {
    }

    public ServicesAPIContext(DbContextOptions<ServicesAPIContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BlacklistedToken> BlacklistedTokens { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectImage> ProjectImages { get; set; }

    public virtual DbSet<ProjectTool> ProjectTools { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    public virtual DbSet<SpotifyAccount> SpotifyAccounts { get; set; }

    public virtual DbSet<SpotifyAccountRequest> SpotifyAccountRequests { get; set; }

    public virtual DbSet<SpotifyAlbum> SpotifyAlbums { get; set; }

    public virtual DbSet<SpotifyArtist> SpotifyArtists { get; set; }

    public virtual DbSet<SpotifyArtistAlbum> SpotifyArtistAlbums { get; set; }

    public virtual DbSet<SpotifySession> SpotifySessions { get; set; }

    public virtual DbSet<SpotifySong> SpotifySongs { get; set; }

    public virtual DbSet<SpotifySongAlbum> SpotifySongAlbums { get; set; }

    public virtual DbSet<SpotifySongArtist> SpotifySongArtists { get; set; }

    public virtual DbSet<SpotifyTrackPlay> SpotifyTrackPlays { get; set; }

    public virtual DbSet<Tool> Tools { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAccountRequest> UserAccountRequests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlacklistedToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__Blacklis__658FEEEA46DDFCC9");

            entity.ToTable("BlacklistedToken");

            entity.Property(e => e.TokenData)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.TokenType)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.BlacklistedTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Token_FK_User");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC070DF849C7");

            entity.ToTable("Category");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Educatio__3214EC0793880FF1");

            entity.ToTable("Education");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Gpa)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("GPA");
            entity.Property(e => e.GraduationDate).HasColumnType("date");
            entity.Property(e => e.Major)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RewardType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SchoolName)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.SchoolType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SchoolUrl)
                .HasMaxLength(1023)
                .IsUnicode(false)
                .HasColumnName("SchoolURL");

            entity.HasOne(d => d.Image).WithMany(p => p.Educations)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("FK_Education_Image");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Image__3214EC074DC14802");

            entity.ToTable("Image");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.FileName)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.Key)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(1023)
                .IsUnicode(false)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Position__3214EC07A7C8460C");

            entity.ToTable("Position");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.CompanyUrl)
                .HasMaxLength(1023)
                .IsUnicode(false)
                .HasColumnName("CompanyURL");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("date");

            entity.HasOne(d => d.Image).WithMany(p => p.Positions)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("FK_Position_Image");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Project__3214EC0798D4C98A");

            entity.ToTable("Project");

            entity.HasIndex(e => e.Name, "UQ__Project__737584F62DF69406").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.RepoUrl)
                .HasMaxLength(511)
                .IsUnicode(false)
                .HasColumnName("RepoURL");
            entity.Property(e => e.Url)
                .HasMaxLength(511)
                .IsUnicode(false)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<ProjectImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectI__3214EC071FFCA234");

            entity.ToTable("ProjectImage");

            entity.HasOne(d => d.Image).WithMany(p => p.ProjectImages)
                .HasForeignKey(d => d.ImageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectImage_Image");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectImages)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectImage_Project");
        });

        modelBuilder.Entity<ProjectTool>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectT__3214EC0727C3B455");

            entity.ToTable("ProjectTool");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTools)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectTool_Project");

            entity.HasOne(d => d.Tool).WithMany(p => p.ProjectTools)
                .HasForeignKey(d => d.ToolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectTool_Tool");
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Resume__3214EC07E42E64B0");

            entity.ToTable("Resume");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreationDate).HasColumnType("date");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Key)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(1023)
                .IsUnicode(false)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<SpotifyAccount>(entity =>
        {
            entity.HasKey(e => e.SpotifyAuthId).HasName("PK__SpotifyA__F8180AFF78DA5EDE");

            entity.ToTable("SpotifyAccount");

            entity.Property(e => e.AccessGeneratedDate).HasColumnType("datetime");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.RefreshGeneratedDate).HasColumnType("datetime");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(511)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.SpotifyAccounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SpotifyAccount_FK_User");
        });

        modelBuilder.Entity<SpotifyAccountRequest>(entity =>
        {
            entity.HasKey(e => e.SpotifyAccountRequestId).HasName("PK__tmp_ms_x__9436FFFE3431E3FF");

            entity.ToTable("SpotifyAccountRequest");

            entity.Property(e => e.AuthorizationCode).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.OriginalUrl)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.SpotifyAccountRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SpotifyAccountRequest_FK_User");
        });

        modelBuilder.Entity<SpotifyAlbum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpotifyA__3214EC071938DE0D");

            entity.ToTable("SpotifyAlbum");

            entity.Property(e => e.Id)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ArtworkUrl)
                .HasMaxLength(1023)
                .IsUnicode(false)
                .HasColumnName("ArtworkURL");
            entity.Property(e => e.Title)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(511)
                .IsUnicode(false)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<SpotifyArtist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpotifyA__3214EC07BDD9C7A5");

            entity.ToTable("SpotifyArtist");

            entity.Property(e => e.Id)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(511)
                .IsUnicode(false)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<SpotifyArtistAlbum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpotifyA__3214EC074DFC1032");

            entity.ToTable("SpotifyArtistAlbum");

            entity.Property(e => e.AlbumId)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ArtistId)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Album).WithMany(p => p.SpotifyArtistAlbums)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArtistAlbum_Album");

            entity.HasOne(d => d.Artist).WithMany(p => p.SpotifyArtistAlbums)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArtistAlbum_Artist");
        });

        modelBuilder.Entity<SpotifySession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpotifyS__3214EC072E10A088");

            entity.ToTable("SpotifySession");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.SpotifySessions)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Session_SpotifyAccount");
        });

        modelBuilder.Entity<SpotifySong>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpotifyS__3214EC07738F5957");

            entity.ToTable("SpotifySong");

            entity.Property(e => e.Id)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(511)
                .IsUnicode(false)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<SpotifySongAlbum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpotifyS__3214EC07C589D428");

            entity.ToTable("SpotifySongAlbum");

            entity.Property(e => e.AlbumId)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SongId)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Album).WithMany(p => p.SpotifySongAlbums)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SongAlbum_Album");

            entity.HasOne(d => d.Song).WithMany(p => p.SpotifySongAlbums)
                .HasForeignKey(d => d.SongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SongAlbum_Song");
        });

        modelBuilder.Entity<SpotifySongArtist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpotifyS__3214EC076BF65C4C");

            entity.Property(e => e.ArtistId)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SongId)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Artist).WithMany(p => p.SpotifySongArtists)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SongArtist_Artist");

            entity.HasOne(d => d.Song).WithMany(p => p.SpotifySongArtists)
                .HasForeignKey(d => d.SongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SongArtist_Song");
        });

        modelBuilder.Entity<SpotifyTrackPlay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpotifyT__3214EC078B574A52");

            entity.ToTable("SpotifyTrackPlay");

            entity.Property(e => e.SongId)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Session).WithMany(p => p.SpotifyTrackPlays)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TrackPlay_Session");

            entity.HasOne(d => d.Song).WithMany(p => p.SpotifyTrackPlays)
                .HasForeignKey(d => d.SongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TrackPlay_Song");
        });

        modelBuilder.Entity<Tool>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tool__3214EC073E7C40A7");

            entity.ToTable("Tool");

            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(1023)
                .IsUnicode(false)
                .HasColumnName("URL");

            entity.HasOne(d => d.Category).WithMany(p => p.Tools)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Tool_Category");

            entity.HasOne(d => d.Image).WithMany(p => p.Tools)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("FK_Tool_Image");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C51F4112A");

            entity.ToTable("User");

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Uuid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("UUID");
        });

        modelBuilder.Entity<UserAccountRequest>(entity =>
        {
            entity.HasKey(e => e.UserAccountRequestId).HasName("PK__UserAcco__5E22BAD676879072");

            entity.ToTable("UserAccountRequest");

            entity.Property(e => e.GeneratedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RequestCode).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.User).WithMany(p => p.UserAccountRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserAccountRequest_FK_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
