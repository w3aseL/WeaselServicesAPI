using System;
using System.Collections.Generic;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer;

public partial class ServicesAPIContext : DbContext
{
    public ServicesAPIContext(DbContextOptions<ServicesAPIContext> options)
        : base(options)
    {
        if (!IsInitialized)
            { IsInitialized = true; InitializeAzureKeyVaultProvider(); }
    }

    public virtual DbSet<AuthenticationMethod> AuthenticationMethods { get; set; }

    public virtual DbSet<BlacklistedToken> BlacklistedTokens { get; set; }

    public virtual DbSet<BlogAuthor> BlogAuthors { get; set; }

    public virtual DbSet<BlogCategory> BlogCategories { get; set; }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Link> Links { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectImage> ProjectImages { get; set; }

    public virtual DbSet<ProjectTool> ProjectTools { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SpotifyAccount> SpotifyAccounts { get; set; }

    public virtual DbSet<SpotifyAccountRequest> SpotifyAccountRequests { get; set; }

    public virtual DbSet<SpotifyAlbum> SpotifyAlbums { get; set; }

    public virtual DbSet<SpotifyArtist> SpotifyArtists { get; set; }

    public virtual DbSet<SpotifyArtistAlbum> SpotifyArtistAlbums { get; set; }

    public virtual DbSet<SpotifyArtistGenre> SpotifyArtistGenres { get; set; }

    public virtual DbSet<SpotifyGenre> SpotifyGenres { get; set; }

    public virtual DbSet<SpotifySession> SpotifySessions { get; set; }

    public virtual DbSet<SpotifySong> SpotifySongs { get; set; }

    public virtual DbSet<SpotifySongAlbum> SpotifySongAlbums { get; set; }

    public virtual DbSet<SpotifySongArtist> SpotifySongArtists { get; set; }

    public virtual DbSet<SpotifyTrackPlay> SpotifyTrackPlays { get; set; }

    public virtual DbSet<Tool> Tools { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAccountRequest> UserAccountRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Scaffolding:ConnectionString", "Data Source=(local);Initial Catalog=Database;Integrated Security=true");

        modelBuilder.Entity<AuthenticationMethod>(entity =>
        {
            entity.ToTable("AuthenticationMethod");

            entity.HasIndex(e => e.UserId, "IDX_AuthenticationMethod_UserId");

            entity.Property(e => e.AuthenticationTypeId).HasDefaultValueSql("1");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SecretKey)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.AuthenticationMethods)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuthenticationMethod_User");
        });

        modelBuilder.Entity<BlacklistedToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);

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

        modelBuilder.Entity<BlogAuthor>(entity =>
        {
            entity.ToTable("BlogAuthor");

            entity.HasIndex(e => e.UserId, "IDX_Author_Account");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.BlogAuthors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Author_User");
        });

        modelBuilder.Entity<BlogCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.ToTable("BlogCategory");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.TagColor)
                .HasMaxLength(31)
                .IsUnicode(false)
                .HasDefaultValueSql("'ffffff'");
            entity.Property(e => e.TagIcon)
                .HasMaxLength(63)
                .IsUnicode(false);
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.ToTable("BlogPost");

            entity.HasIndex(e => e.AuthorId, "IDX_BlogPost_Author");

            entity.Property(e => e.BlogContent).IsUnicode(false);
            entity.Property(e => e.BlogDescription).IsUnicode(false);
            entity.Property(e => e.BlogTitle)
                .HasMaxLength(1024)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DatePublished).HasColumnType("datetime");
            entity.Property(e => e.LastModified).HasColumnType("datetime");

            entity.HasOne(d => d.Author).WithMany(p => p.BlogPosts)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_BlogPost_Author");

            entity.HasMany(d => d.BlogCategories).WithMany(p => p.BlogPosts)
                .UsingEntity<Dictionary<string, object>>(
                    "BlogPostCategory",
                    r => r.HasOne<BlogCategory>().WithMany()
                        .HasForeignKey("BlogCategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BlogPostCategory_BlogCategory"),
                    l => l.HasOne<BlogPost>().WithMany()
                        .HasForeignKey("BlogPostId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BlogPostCategory_BlogPost"),
                    j =>
                    {
                        j.HasKey("BlogPostId", "BlogCategoryId");
                        j.ToTable("BlogPostCategory");
                    });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("Device");

            entity.Property(e => e.DeviceId)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.DeviceIpaddress)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DeviceIPAddress");
            entity.Property(e => e.DeviceName)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.Manufacturer)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.Uuid)
                .HasDefaultValueSql("newid()")
                .HasColumnName("UUID");
        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.ToTable("Education");

            entity.Property(e => e.Id).HasDefaultValueSql("newid()");
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
            entity.ToTable("Image");

            entity.Property(e => e.Id).HasDefaultValueSql("newid()");
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

        modelBuilder.Entity<Link>(entity =>
        {
            entity.ToTable("Link");

            entity.Property(e => e.LinkName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LinkUrl)
                .HasMaxLength(2047)
                .IsUnicode(false)
                .HasColumnName("LinkURL");
            entity.Property(e => e.LogoAlt)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(2047)
                .IsUnicode(false)
                .HasColumnName("LogoURL");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permission");

            entity.Property(e => e.Name)
                .HasMaxLength(511)
                .IsUnicode(false);

            entity.HasMany(d => d.Roles).WithMany(p => p.Permissions)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolePermission_Role"),
                    l => l.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolePermission_Permission"),
                    j =>
                    {
                        j.HasKey("PermissionId", "RoleId");
                        j.ToTable("RolePermission");
                    });
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable("Position");

            entity.Property(e => e.Id).HasDefaultValueSql("newid()");
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
            entity.ToTable("Project");

            entity.HasIndex(e => e.Name, "IX_Project_Name").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("newid()");
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
            entity.ToTable("Resume");

            entity.Property(e => e.Id).HasDefaultValueSql("newid()");
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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.RoleName)
                .HasMaxLength(511)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SpotifyAccount>(entity =>
        {
            entity.HasKey(e => e.SpotifyAuthId);

            entity.ToTable("SpotifyAccount");

            entity.HasIndex(e => e.UserId, "IDX_SpotifyAccount_User");

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
            entity.ToTable("SpotifyAccountRequest");

            entity.Property(e => e.AuthorizationCode).HasDefaultValueSql("newid()");
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
            entity.ToTable("SpotifyArtistAlbum");

            entity.HasIndex(e => e.AlbumId, "IDX_ArtistAlbum_Album");

            entity.HasIndex(e => e.ArtistId, "IDX_ArtistAlbum_Artist");

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

        modelBuilder.Entity<SpotifyArtistGenre>(entity =>
        {
            entity.ToTable("SpotifyArtistGenre");

            entity.HasIndex(e => e.GenreId, "IDX_SpotifyArtistGenre_SpotifySong");

            entity.Property(e => e.ArtistId)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Artist).WithMany(p => p.SpotifyArtistGenres)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SpotifyArtistGenre_SpotifySong");

            entity.HasOne(d => d.Genre).WithMany(p => p.SpotifyArtistGenres)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SpotifyArtistGenre_SpotifyGenre");
        });

        modelBuilder.Entity<SpotifyGenre>(entity =>
        {
            entity.ToTable("SpotifyGenre");

            entity.Property(e => e.Name)
                .HasMaxLength(511)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SpotifySession>(entity =>
        {
            entity.ToTable("SpotifySession");

            entity.HasIndex(e => e.AccountId, "IDX_SpotifySession_Account");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.SpotifySessions)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Session_SpotifyAccount");
        });

        modelBuilder.Entity<SpotifySong>(entity =>
        {
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
            entity.ToTable("SpotifySongAlbum");

            entity.HasIndex(e => e.AlbumId, "IDX_SongAlbum_Album");

            entity.HasIndex(e => e.SongId, "IDX_SongAlbum_Song");

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
            entity.HasIndex(e => e.ArtistId, "IDX_SongArtist_Artist");

            entity.HasIndex(e => e.SongId, "IDX_SongArtist_Song");

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
            entity.ToTable("SpotifyTrackPlay");

            entity.HasIndex(e => e.SessionId, "IDX_TrackPlay_Session");

            entity.HasIndex(e => e.SongId, "IDX_TrackPlay_Song");

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
            entity.ToTable("User");

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Uuid)
                .HasDefaultValueSql("newid()")
                .HasColumnName("UUID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<UserAccountRequest>(entity =>
        {
            entity.ToTable("UserAccountRequest");

            entity.Property(e => e.GeneratedDate)
                .HasDefaultValueSql("getdate()")
                .HasColumnType("datetime");
            entity.Property(e => e.RequestCode).HasDefaultValueSql("newid()");

            entity.HasOne(d => d.User).WithMany(p => p.UserAccountRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserAccountRequest_FK_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
