public class InsulationCalculator
{
    public double CalculateArea(int diameter, int thickness, int pipeLength, int layerMultiplier)
    {
        double layer = CalculateLayerArea(diameter, thickness, layerMultiplier);
        return pipeLength * (layer / 1000);
    }

    public double CalculateLayerArea(int diameter, int thickness, int layerMultiplier)
    {
        return (diameter + (thickness * layerMultiplier)) * Math.PI;
    }

    public double CalculateFirstLayerArea(int diameter, int thickness, int pipeLength)
    {
        return CalculateArea(diameter, thickness, pipeLength, 2);
    }

    public double CalculateSecondLayerArea(int diameter, int thickness, int pipeLength)
    {
        return CalculateArea(diameter, thickness, pipeLength, 4);
    }

    public double CalculateTotalArea(double firstLayerSqm, double secondLayerSqm)
    {
        return firstLayerSqm + secondLayerSqm;
    }

    public double CalculateRolls(double area, double rollArea)
    {
        return area / rollArea;
    }

    public double CalculateTotalRolls(double rollsFirstLayer, double RollsSecondLayer)
    {
        return rollsFirstLayer + RollsSecondLayer;
    }

    public static double CalculateRectangularPerimeter(double sideA, double sideB, double extraThickness)
    {
        return 2 * (sideA + extraThickness) + 2 * (sideB + extraThickness);
    }

    public static double CalculateRectangularFirstLayerArea(double sideA, double sideB, double thickness, double pipeLength)
    {
        double perimeter = CalculateRectangularPerimeter(sideA, sideB, thickness);
        return perimeter * pipeLength;
    }

    public static double CalculateRectangularSecondLayerArea(double sideA, double sideB, double firstThickness, double secondThickness, double pipeLength)
    {
        double totalThickness = firstThickness + secondThickness;
        double perimeter = CalculateRectangularPerimeter(sideA, sideB, totalThickness);
        return perimeter * pipeLength;
    }
}