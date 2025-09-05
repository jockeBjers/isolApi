namespace IsolCore.Data;

using Microsoft.EntityFrameworkCore;
using IsolCore.Models;

public class Database : DbContext, IDatabase
{
    public Database(DbContextOptions<Database> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<InsulatedPipeBase> Pipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>().OwnsOne(u => u.RefreshToken);

        modelBuilder.Entity<InsulatedPipeBase>()
            .HasDiscriminator<string>("PipeType")
            .HasValue<CircularInsulatedPipe>("Circular")
            .HasValue<RectangularInsulatedPipe>("Rectangular");
    }
    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
}
