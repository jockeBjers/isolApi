namespace IsolCore.Data;

using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IsolCore.Models;

public interface IDatabase
{
    DbSet<User> Users { get; set; }
    DbSet<Organization> Organizations { get; set; }
    DbSet<Project> Projects { get; set; }

    DbSet<InsulatedPipeBase> Pipes { get; set; }
    DbSet<InsulationType> InsulationTypes { get; set; }
    Task<int> SaveChangesAsync();
}
