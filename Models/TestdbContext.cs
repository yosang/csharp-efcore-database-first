using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace test.Models;

public partial class TestdbContext : DbContext
{
    public TestdbContext()
    {
    }

    public TestdbContext(DbContextOptions<TestdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Tool> Tools { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySQL("server=localhost;database=testdb;user=testuser;password=p@ssword");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<Tool>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.BrandId, "IX_Tools_BrandID");

            entity.HasIndex(e => e.CategoryId, "IX_Tools_CategoryID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

            entity.HasOne(d => d.Brand).WithMany(p => p.Tools).HasForeignKey(d => d.BrandId);

            entity.HasOne(d => d.Category).WithMany(p => p.Tools).HasForeignKey(d => d.CategoryId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
