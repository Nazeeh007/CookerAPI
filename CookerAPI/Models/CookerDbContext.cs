using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CookerAPI.Models;

public partial class CookerDbContext : DbContext
{

	public CookerDbContext(DbContextOptions<CookerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersView> OrdersViews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TotalOrder> TotalOrders { get; set; }

    public virtual DbSet<UserPass> UserPasses { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserRolesName> UserRolesNames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Gimage).HasColumnName("GImage");
            entity.Property(e => e.Gname)
                .HasMaxLength(100)
                .HasColumnName("GName");
            entity.Property(e => e.Gstatus).HasColumnName("GStatus");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItmDesc).HasMaxLength(150);
            entity.Property(e => e.ItmName).HasMaxLength(100);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OrdersView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OrdersView");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<TotalOrder>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
        });

        modelBuilder.Entity<UserPass>(entity =>
        {
            entity.ToTable("UserPass");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmployeeName).HasMaxLength(50);
            entity.Property(e => e.Pass).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<UserRolesName>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("UserRolesName");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
