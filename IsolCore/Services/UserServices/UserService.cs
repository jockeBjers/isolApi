
namespace IsolCore.Services.UserServices;

using Microsoft.EntityFrameworkCore;
using IsolCore.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserService(IDatabase DbContext) : IUserService
{
    private readonly IDatabase _db = DbContext;


    public async Task<List<User>> GetAllUsers()
    {
        var users = await _db.Users
            .ToListAsync();

        return users;
    }

    public async Task<bool> DoesUserExist(string username)
    {
        return await _db.Users.AnyAsync(u => u.Name == username);
    }

    public async Task<User?> GetUserByName(string username)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Name == username);

        if (user == null)
            return null;

        return user;
    }

    public async Task<User?> GetUserById(int userId)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return null;

        return user;
    }

    public async Task AddUser(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> RemoveUserById(int userId)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return false;

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<User?> UpdateUserAdmin(int userId, User updatedUser)
    {
        var existingUser = await _db.Users.FindAsync(userId);
        if (existingUser == null)
            return null;

        existingUser.Name = updatedUser.Name;
        existingUser.Email = updatedUser.Email;
        existingUser.Phone = updatedUser.Phone;
        existingUser.OrganizationId = updatedUser.OrganizationId;
        existingUser.Role = updatedUser.Role;

        await _db.SaveChangesAsync();

        return existingUser;
    }

    public async Task<User?> UpdateUser(int userId, User updatedUser)
    {
        var existingUser = await _db.Users.FindAsync(userId);
        if (existingUser == null)
            return null;

        if(!String.IsNullOrEmpty(updatedUser.PasswordHash))
            existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUser.PasswordHash);

        existingUser.Email = updatedUser.Email;
        existingUser.Phone = updatedUser.Phone;

        await _db.SaveChangesAsync();

        return existingUser;
    }
}
