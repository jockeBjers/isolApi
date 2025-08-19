
namespace IsolCore.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string OrganizationId { get; set; }
    public string? Phone { get; set; }
    public string Role { get; set; } = "User";
    [Required]
    public required string PasswordHash { get; set; }

    [ForeignKey("OrganizationId")]
    public virtual Organization? Organization { get; set; }

    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
}
