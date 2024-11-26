using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<users>(entity =>
        {
            entity.HasIndex(e => e.username).IsUnique();
            entity.HasIndex(e => e.email).IsUnique();
        });
    }
}