namespace IsolCore.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Project
{
    [Key]
    public int Id { get; set; }
    [Required]
    public required string ProjectNumber { get; set; }
    [Required]
    public required string Name { get; set; }
    public DateTime Date { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    [Required]
    public required string OrganizationId { get; set; }
    public string? Address { get; set; }
    public string? Customer { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactNumber { get; set; }
    public string? Comment { get; set; }

    [ForeignKey("OrganizationId")]
    public virtual Organization? Organization { get; set; }

    public ICollection<InsulatedPipeBase> Pipes { get; set; } = new List<InsulatedPipeBase>();

}
