using System.ComponentModel.DataAnnotations.Schema;
public class CircularInsulatedPipe : InsulatedPipeBase
{
    public int? SizeId { get; set; }

    [NotMapped]
    public CircularPipeSize? Size
    {
        get => SizeId.HasValue ? CircularPipeSize.StandardSizes.FirstOrDefault(s => s.Id == SizeId.Value) : null;
        set => SizeId = value?.Id;
    }

    public CircularInsulatedPipe() { }

    public CircularInsulatedPipe(int id, int? sizeId, double length, InsulationType firstLayerMaterial, InsulationType? secondLayerMaterial, string projectNumber)
    {
        Id = id;
        SizeId = sizeId;
        Length = length;
        FirstLayerMaterial = firstLayerMaterial;
        SecondLayerMaterial = secondLayerMaterial;
        ProjectNumber = projectNumber;
    }

    private readonly InsulationCalculator _calculator = new InsulationCalculator();

    public override double GetFirstLayerArea()
    {
        var size = Size;
        if (size == null) return 0;
        return _calculator.CalculateFirstLayerArea(
            (int)(size.Diameter * 1000),
            (int)FirstLayerMaterial.InsulationThickness,
            (int)Length
        );
    }

    public override double GetSecondLayerArea()
    {
        var size = Size;
        if (SecondLayerMaterial == null || size == null) return 0;
        return _calculator.CalculateSecondLayerArea(
            (int)(size.Diameter * 1000),
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