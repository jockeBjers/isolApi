using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class InsulationType
{
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    public double InsulationThickness { get; set; }
    public double InsulationAreaPerMeter { get; set; }
    public string InsulationCategory { get; set; } = "heat";

    [ForeignKey("OrganizationId")]
    public string OrganizationId { get; set; }

    public InsulationType() { }

    public InsulationType(int id, string name, double insulationThickness, double insulationAreaPerMeter, string insulationCategory = "heat")
    {
        Id = id;
        Name = name;
        InsulationThickness = insulationThickness;
        InsulationAreaPerMeter = insulationAreaPerMeter;
        InsulationCategory = insulationCategory;
    }

    public static readonly List<InsulationType> DefaultMaterials = new List<InsulationType>
    {
        new(){ Id = 1, Name = "30mm, 3.6m²", InsulationThickness = 0.03, InsulationAreaPerMeter = 3.6, InsulationCategory = "heat" },
        new(){ Id = 2, Name = "50mm, 2.7m²", InsulationThickness = 0.05, InsulationAreaPerMeter = 2.7, InsulationCategory = "heat" },
        new(){ Id = 3, Name = "80mm, 1.5m²", InsulationThickness = 0.08, InsulationAreaPerMeter = 1.5, InsulationCategory = "heat" }
    };

    // Legacy method for backward compatibility
    public static List<InsulationType> GetDefaultTypes()
    {
        return DefaultMaterials;
    }

}
