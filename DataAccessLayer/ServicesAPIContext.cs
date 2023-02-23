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
