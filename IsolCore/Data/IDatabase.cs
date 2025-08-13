namespace IsolCore.Data;

using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IsolCore.Models;

public interface IDatabase
{
    DbSet<User> Users { get; set; }
    DbSet<Organization> Organizations { get; set; }
    Task<int> SaveChangesAsync();
}
