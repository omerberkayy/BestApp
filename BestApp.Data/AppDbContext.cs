using Microsoft.EntityFrameworkCore;
using BestApp.Core.Entities;

namespace BestApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<About> Abouts { get; set; } = null!;
    public DbSet<ContactMessage> ContactMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Veritabanı sütun kısıtlamalarını (Validation) Fluent API ile burada yapılandırabilirsin
        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(150).IsRequired();
            entity.Property(e => e.ImageUrl).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<Service>().Property(s => s.Title).HasMaxLength(100).IsRequired();
        modelBuilder.Entity<ContactMessage>().Property(c => c.Name).HasMaxLength(100).IsRequired();
    }
}