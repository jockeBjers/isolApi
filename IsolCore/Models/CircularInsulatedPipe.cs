using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CircularInsulatedPipe : InsulatedPipeBase
{

    public required CircularPipeSize Size { get; set; }

    public CircularInsulatedPipe() { }

    public CircularInsulatedPipe(int id, CircularPipeSize size, double length, InsulationType firstLayerMaterial, InsulationType? secondLayerMaterial, int projectId)
    {
        Id = id;
        Size = size;
        Length = length;
        FirstLayerMaterial = firstLayerMaterial;
        SecondLayerMaterial = secondLayerMaterial;
        ProjectId = projectId;
    }

    private readonly InsulationCalculator _calculator = new InsulationCalculator();

    public override double GetFirstLayerArea()
    {
        return _calculator.CalculateFirstLayerArea(
            (int)(Size.Diameter * 1000), 
            (int)FirstLayerMaterial.InsulationThickness,
            (int)Length
        );
    }

    public override double GetSecondLayerArea()
    {
        if (SecondLayerMaterial == null) return 0;
        return _calculator.CalculateSecondLayerArea(
            (int)(Size.Diameter * 1000), 
            (int)SecondLayerMaterial.InsulationThickness,
            (int)Length
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