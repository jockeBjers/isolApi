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

        if (!_db.InsulationTypes.Any())
        {
            _db.InsulationTypes.Add(new InsulationType
            {
                Id = 1,
                Name = "30mm, 3.6m²",
                InsulationThickness = 0.03,
                InsulationAreaPerMeter = 3.6,
                InsulationCategory = "heat",
                OrganizationId = "50522"
            });
            _db.InsulationTypes.Add(new InsulationType
            {
                Id = 2,
                Name = "50mm, 2.7m²",
                InsulationThickness = 0.05,
                InsulationAreaPerMeter = 2.7,
                InsulationCategory = "heat",
                OrganizationId = "50522"
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
        if (!_db.Pipes.Any())
        {
            var targetProject = await _db.Projects.FirstOrDefaultAsync(p => p.ProjectNumber == "505052");
            var firstInsulationType = _db.InsulationTypes.FirstOrDefault(t => t.Id == 1);

            if (targetProject != null && firstInsulationType != null)
            {
                var pipe1 = new CircularInsulatedPipe
                {
                    ProjectNumber = "505052",
                    Length = 10,
                    FirstLayerMaterial = firstInsulationType,
                    SizeId = 4,
                };
                var pipe2 = new RectangularInsulatedPipe
                {
                    ProjectNumber = "505052",
                    Length = 10,
                    SideA = 40,
                    SideB = 20,
                    FirstLayerMaterial = firstInsulationType,
                };

                _db.Pipes.Add(pipe1);
                _db.Pipes.Add(pipe2);
                await _db.SaveChangesAsync();

            }
        }
    }
}
