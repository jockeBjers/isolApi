namespace IsolCore.Data;

using System;
using System.Linq;
using System.Threading.Tasks;
using IsolCore.Models;

public class DatabaseInitializer(Database db)
{
    private readonly Database _db = db;

    public async Task InitDatabase()
    {
        // Check if new database was created if so seed it
        if (_db.Database.EnsureCreated())
        {
            await SeedDatabase();
        }
    }

    public async Task ResetDatabase()
    {
        try
        {
            _db.Database.EnsureDeleted();
            _db.Database.EnsureCreated();
            await SeedDatabase();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while resetting database: {ex.Message}");
        }
    }
private async Task SeedDatabase()
{

    if (!_db.Organizations.Any())
    {
        var org = new Organization
        {
            Id = "50522",
            Name = "Taekwando",
            Address = "123 Main St",
            Phone = "011 22 44 00",
            Email = "info@taektaek.com"
        };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();
    }

    if (!_db.Users.Any())
    {
        _db.Users.Add(new User
        {
            Name = "Admin TheMan",
            Email = "admin@example.com",
            OrganizationId = "50522",
            Phone = "011 22 44 00",
            Role = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
        });

        _db.Users.Add(new User
        {
            Name = "User TheLooser",
            Email = "user1@example.com",
            OrganizationId = "50522",
            Phone = "011 22 44 01",
            Role = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
        });

        await _db.SaveChangesAsync(); 
    }
}
}
