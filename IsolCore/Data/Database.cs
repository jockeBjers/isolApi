namespace IsolCore.Data;

using Microsoft.EntityFrameworkCore;
using IsolCore.Models;

public class Database : DbContext, IDatabase
{   
    public Database(DbContextOptions<Database> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
}
