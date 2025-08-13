namespace IsolCore.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Organization
{
    [Key]
    public required string Id { get; set; }
    [Required]
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public string? Website { get; set; }
    
    public List<User> Users { get; set; } = new List<User>();
    public List<Project> Projects { get; set; } = new List<Project>();
}
