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
                Address = "123 Torget",
                Phone = "011 22 44 20",
                Email = "info@taektaek.com"
            };
            var org2 = new Organization
            {
                Id = "50523",
                Name = "Tokens",
                Address = "123 Storgatan",
                Phone = "011 22 44 00",
                Email = "info@taektaek.com"
            };
            _db.Organizations.Add(org);
            _db.Organizations.Add(org2);
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
                Role = "Manager",
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

        if (!_db.Projects.Any())
        {
            var project = new Project
            {
                ProjectNumber = "505052",
                Name = "Västerskolan",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddMonths(6),
                Address = "Skolgatan 12, finspång",
                Customer = "Trello",
                ContactPerson = "Alice",
                ContactNumber = "011 22 44 02",
                Comment = "ring vid ankomst",
                OrganizationId = "50522"
            };
            var project2 = new Project
            {
                ProjectNumber = "505053",
                Name = "Österskolan",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddMonths(6),
                Address = "Skolgatan 12, finspång",
                Customer = "Trello",
                ContactPerson = "Alice",
                ContactNumber = "011 22 44 02",
                Comment = "ring vid ankomst",
                OrganizationId = "50523"
            };
            _db.Projects.Add(project);
            _db.Projects.Add(project2);
            await _db.SaveChangesAsync();
        }
    }
}
