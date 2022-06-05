using System;
using System.Collections.Generic;
using APIWithRabbitMQ.Domain.Models.Entities;
using APIWithRabbitMQ.Domain.Models.Entities.Membership;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace APIWithRabbitMQ.Domain.Models.DataContexts
{
    public class RabbitDbContext :
        IdentityDbContext<AppUser, AppRole, int, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public RabbitDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<AppRole> Roles { get; set; }
        public DbSet<AppRoleClaim> RoleClaims { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppUserRole> UserRoles { get; set; }
        public DbSet<AppUserClaim> UserClaims { get; set; }
        public DbSet<AppUserLogin> UserLogins { get; set; }
        public DbSet<AppUserToken> UserTokens { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<News> News { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=API-With-RabbitMQ;User Id=sa;password=query;MultipleActiveResultSets=true;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.HasMany(e => e.UserRoles)
                      .WithOne(e => e.Role)
                      .HasForeignKey(e => e.RoleId)
                      .IsRequired();

                entity.ToTable("Roles", "Membership");
            });

            modelBuilder.Entity<AppRoleClaim>(entity =>
            {
                entity.ToTable("RoleClaims", "Membership");
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasMany(e => e.UserRoles)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                      .IsRequired();

                entity.ToTable("Users", "Membership");
            });

            modelBuilder.Entity<AppUserRole>(entity =>
            {
                entity.ToTable("UserRoles", "Membership");
            });

            modelBuilder.Entity<AppUserClaim>(entity =>
            {
                entity.ToTable("UserClaims", "Membership");
            });

            modelBuilder.Entity<AppUserLogin>(entity =>
            {
                entity.ToTable("UserLogins", "Membership");
            });

            modelBuilder.Entity<AppUserToken>(entity =>
            {
                entity.ToTable("UserTokens", "Membership");
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(dateadd(hour,(4),getutcdate()))");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(dateadd(hour,(4),getutcdate()))");
            });
        }
    }
}
