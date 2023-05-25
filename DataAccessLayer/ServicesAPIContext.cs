﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DataAccessLayer.Models;

namespace DataAccessLayer
{
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

            modelBuilder.Entity<BlacklistedToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.ToTable("BlacklistedToken");

                entity.Property(e => e.TokenData)
                    .IsRequired()
                    .HasMaxLength(511)
                    .IsUnicode(false);

                entity.Property(e => e.TokenType)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BlacklistedTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Token_FK_User");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
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
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SchoolName)
                    .IsRequired()
                    .HasMaxLength(511)
                    .IsUnicode(false);

                entity.Property(e => e.SchoolType)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SchoolUrl)
                    .IsRequired()
                    .HasMaxLength(1023)
                    .IsUnicode(false)
                    .HasColumnName("SchoolURL");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Educations)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Education_Image");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.Id).HasDefaultValueSql("newid()");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(511)
                    .IsUnicode(false);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(511)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(1023)
                    .IsUnicode(false)
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(511)
                    .IsUnicode(false);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Permissions)
                    .UsingEntity<Dictionary<string, object>>(
                        "RolePermission",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_RolePermission_Role"),
                        r => r.HasOne<Permission>().WithMany().HasForeignKey("PermissionId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_RolePermission_Permission"),
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
                    .IsRequired()
                    .HasMaxLength(511)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyUrl)
                    .IsRequired()
                    .HasMaxLength(1023)
                    .IsUnicode(false)
                    .HasColumnName("CompanyURL");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.JobTitle)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Positions)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Position_Image");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project");

                entity.HasIndex(e => e.Name, "IX_Project_Name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("newid()");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
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

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ProjectImages)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectImage_Image");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectImages)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectImage_Project");
            });

            modelBuilder.Entity<ProjectTool>(entity =>
            {
                entity.ToTable("ProjectTool");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectTools)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectTool_Project");

                entity.HasOne(d => d.Tool)
                    .WithMany(p => p.ProjectTools)
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
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(1023)
                    .IsUnicode(false)
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(511)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SpotifyAccount>(entity =>
            {
                entity.HasKey(e => e.SpotifyAuthId);

                entity.ToTable("SpotifyAccount");

                entity.Property(e => e.AccessGeneratedDate).HasColumnType("datetime");

                entity.Property(e => e.AccessToken)
                    .IsRequired()
                    .HasMaxLength(511)
                    .IsUnicode(false);

                entity.Property(e => e.RefreshGeneratedDate).HasColumnType("datetime");

                entity.Property(e => e.RefreshToken)
                    .IsRequired()
                    .HasMaxLength(511)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SpotifyAccounts)
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

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SpotifyAccountRequests)
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
                    .IsRequired()
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
                    .IsRequired()
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

                entity.Property(e => e.AlbumId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ArtistId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.SpotifyArtistAlbums)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArtistAlbum_Album");

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.SpotifyArtistAlbums)
                    .HasForeignKey(d => d.ArtistId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArtistAlbum_Artist");
            });

            modelBuilder.Entity<SpotifySession>(entity =>
            {
                entity.ToTable("SpotifySession");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.SpotifySessions)
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
                    .IsRequired()
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

                entity.Property(e => e.AlbumId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SongId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.SpotifySongAlbums)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SongAlbum_Album");

                entity.HasOne(d => d.Song)
                    .WithMany(p => p.SpotifySongAlbums)
                    .HasForeignKey(d => d.SongId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SongAlbum_Song");
            });

            modelBuilder.Entity<SpotifySongArtist>(entity =>
            {
                entity.Property(e => e.ArtistId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SongId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.SpotifySongArtists)
                    .HasForeignKey(d => d.ArtistId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SongArtist_Artist");

                entity.HasOne(d => d.Song)
                    .WithMany(p => p.SpotifySongArtists)
                    .HasForeignKey(d => d.SongId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SongArtist_Song");
            });

            modelBuilder.Entity<SpotifyTrackPlay>(entity =>
            {
                entity.ToTable("SpotifyTrackPlay");

                entity.Property(e => e.SongId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.SpotifyTrackPlays)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrackPlay_Session");

                entity.HasOne(d => d.Song)
                    .WithMany(p => p.SpotifyTrackPlays)
                    .HasForeignKey(d => d.SongId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrackPlay_Song");
            });

            modelBuilder.Entity<Tool>(entity =>
            {
                entity.ToTable("Tool");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(1023)
                    .IsUnicode(false)
                    .HasColumnName("URL");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Tools)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Tool_Category");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Tools)
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
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Uuid)
                    .HasColumnName("UUID")
                    .HasDefaultValueSql("newid()");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_Role");
            });

            modelBuilder.Entity<UserAccountRequest>(entity =>
            {
                entity.ToTable("UserAccountRequest");

                entity.Property(e => e.GeneratedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.RequestCode).HasDefaultValueSql("newid()");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAccountRequests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserAccountRequest_FK_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}