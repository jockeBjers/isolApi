
namespace IsolCore.Services.UserServices;

using Microsoft.EntityFrameworkCore;
using IsolCore.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IsolCore.Services.OrganizationServices;

public class OrganizationService(IDatabase DbContext) : IOrganizationService
{
    private readonly IDatabase _db = DbContext;

    public async Task<List<Organization>> GetAllOrganizations()
    {

        return await _db.Organizations.ToListAsync();
    }

    public async Task<Organization?> GetOrganizationById(string organizationId)
    {
        return await _db.Organizations
       .Include(o => o.Users)
       .Include(o => o.Projects)
       .Include(o => o.InsulationTypes)
       .FirstOrDefaultAsync(o => o.Id == organizationId);
    }

    public async Task<bool> DoesOrganizationExist(string organizationId)
    {
        return await _db.Organizations.AnyAsync(o => o.Id == organizationId);
    }

    public async Task AddOrganization(Organization organization)
    {
        _db.Organizations.Add(organization);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> RemoveOrganizationById(string organizationId)
    {
        var organization = await GetOrganizationById(organizationId);
        if (organization == null) return false;

        _db.Organizations.Remove(organization);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Organization?> UpdateOrganization(string organizationId, Organization updatedOrganization)
    {
        var organization = await GetOrganizationById(organizationId);
        if (organization == null) return null;

        organization.Name = updatedOrganization.Name;
        organization.Address = updatedOrganization.Address;
        organization.Phone = updatedOrganization.Phone;
        organization.Email = updatedOrganization.Email;

        await _db.SaveChangesAsync();
        return organization;
    }

    public async Task<User?> GetUserWithOrganization(string userId)
    {
        if (int.TryParse(userId, out int userIdInt))
        {
            return await _db.Users
                .Include(u => u.Organization)
                .FirstOrDefaultAsync(u => u.Id == userIdInt);
        }
        return null;
    }

    


}
