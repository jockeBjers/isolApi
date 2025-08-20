namespace IsolCore.Data;

using Microsoft.EntityFrameworkCore;
using IsolCore.Models;

public class Database : DbContext, IDatabase
{
    public Database(DbContextOptions<Database> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>().OwnsOne(u => u.RefreshToken);

    }
    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
}
