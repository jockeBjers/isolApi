using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace IsolCore.Models;

public abstract class InsulatedPipeBase
{
    [Key]
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public required Project Project { get; set; }
    public double Length { get; set; }
    public required InsulationType FirstLayerMaterial { get; set; }
    public InsulationType? SecondLayerMaterial { get; set; }

    public abstract double GetFirstLayerArea();
    public abstract double GetSecondLayerArea();
    public abstract double GetTotalArea();
    public abstract double GetFirstLayerRolls();
    public abstract double GetSecondLayerRolls();
    public abstract double GetTotalRolls();
}
