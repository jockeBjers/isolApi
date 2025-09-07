
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RectangularInsulatedPipe : InsulatedPipeBase
{
    public double SideA { get; set; }
    public double SideB { get; set; }

    private readonly InsulationCalculator _calculator = new InsulationCalculator();
    
    public RectangularInsulatedPipe() { }

    public RectangularInsulatedPipe(
        double length,
        double sideA,
        double sideB,
        string projectNumber,
        InsulationType firstLayerMaterial,
        InsulationType? secondLayerMaterial = null,
        int id = 0
    )
    {
        Id = id;
        Length = length;
        SideA = sideA;
        SideB = sideB;
        ProjectNumber = projectNumber;
        FirstLayerMaterial = firstLayerMaterial;
        SecondLayerMaterial = secondLayerMaterial;
    }

    public override double GetFirstLayerArea()
    {
        return InsulationCalculator.CalculateRectangularFirstLayerArea(
            SideA, SideB, FirstLayerMaterial.InsulationThickness, Length);
    }

    public override double GetSecondLayerArea()
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

    public override double GetTotalArea()
    {
        return GetFirstLayerArea() + GetSecondLayerArea();
    }

    public override double GetFirstLayerRolls()
    {
        return _calculator.CalculateRolls(
            GetFirstLayerArea(),
            FirstLayerMaterial.InsulationAreaPerMeter
        );
    }

    public override double GetSecondLayerRolls()
    {
        if (SecondLayerMaterial == null) return 0;
        return _calculator.CalculateRolls(
            GetSecondLayerArea(),
            SecondLayerMaterial.InsulationAreaPerMeter
        );
    }

    public override double GetTotalRolls()
    {
        return GetFirstLayerRolls() + GetSecondLayerRolls();
    }
}