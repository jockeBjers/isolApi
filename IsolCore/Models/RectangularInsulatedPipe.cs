
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RectangularInsulatedPipe : InsulatedPipeBase
{
    [Key]
    public int Id { get; set; }
    public double Length { get; set; }
    public double SideA { get; set; }
    public double SideB { get; set; }
    [Required]
    public int ProjectId { get; set; }
    public required InsulationType FirstLayerMaterial { get; set; }
    public InsulationType? SecondLayerMaterial { get; set; }
    
    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    private readonly InsulationCalculator _calculator = new InsulationCalculator();
    
    public RectangularInsulatedPipe() { }

    public RectangularInsulatedPipe(
        double length,
        double sideA,
        double sideB,
        int projectId,
        InsulationType firstLayerMaterial,
        InsulationType? secondLayerMaterial = null,
        int id = 0
    )
    {
        Id = id;
        Length = length;
        SideA = sideA;
        SideB = sideB;
        ProjectId = projectId;
        FirstLayerMaterial = firstLayerMaterial;
        SecondLayerMaterial = secondLayerMaterial;
    }

    public double GetFirstLayerArea()
    {
        return InsulationCalculator.CalculateRectangularFirstLayerArea(
            SideA, SideB, FirstLayerMaterial.InsulationThickness, Length);
    }

    public double GetSecondLayerArea()
    {
        if (SecondLayerMaterial == null) return 0;
        return InsulationCalculator.CalculateRectangularSecondLayerArea(
            SideA,
            SideB,
            FirstLayerMaterial.InsulationThickness,
            SecondLayerMaterial.InsulationThickness,
            Length
        );
    }

    public double GetTotalArea()
    {
        return GetFirstLayerArea() + GetSecondLayerArea();
    }

    public double GetFirstLayerRolls()
    {
        return _calculator.CalculateRolls(
            GetFirstLayerArea(),
            FirstLayerMaterial.InsulationAreaPerMeter
        );
    }

    public double GetSecondLayerRolls()
    {
        if (SecondLayerMaterial == null) return 0;
        return _calculator.CalculateRolls(
            GetSecondLayerArea(),
            SecondLayerMaterial.InsulationAreaPerMeter
        );
    }

    public double GetTotalRolls()
    {
        return GetFirstLayerRolls() + GetSecondLayerRolls();
    }
}