using System;
using IsolCore.Interfaces;
public class CircularInsulatedPipe : InsulatedPipeBase
{
    public int Id { get; set; }
    public CircularPipeSize Size { get; set; }
    public double Length { get; set; }
    public InsulationType FirstLayerMaterial { get; set; }
    public InsulationType? SecondLayerMaterial { get; set; }
    public string ProjectId { get; set; }

    public CircularInsulatedPipe(int id, CircularPipeSize size, double length, InsulationType firstLayerMaterial, InsulationType? secondLayerMaterial, string projectId)
    {
        Id = id;
        Size = size;
        Length = length;
        FirstLayerMaterial = firstLayerMaterial;
        SecondLayerMaterial = secondLayerMaterial;
        ProjectId = projectId;
    }

    private readonly InsulationCalculator _calculator = new InsulationCalculator();

    public double GetFirstLayerArea()
    {
        return _calculator.CalculateFirstLayerArea(
            (int)(Size.Diameter * 1000), 
            (int)FirstLayerMaterial.InsulationThickness,
            (int)Length
        );
    }

    public double GetSecondLayerArea()
    {
        if (SecondLayerMaterial == null) return 0;
        return _calculator.CalculateSecondLayerArea(
            (int)(Size.Diameter * 1000), 
            (int)SecondLayerMaterial.InsulationThickness,
            (int)Length
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