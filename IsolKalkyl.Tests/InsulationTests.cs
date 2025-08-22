namespace Insulation.Tests;
public class InsulationTests
{

    private readonly InsulationCalculator _calculator;

    public InsulationTests()
    {
        _calculator = new InsulationCalculator();
    }

    [Fact]
    public void CalculateFirstLayerArea_ShouldReturnCorrectValue()
    {
        int diameter = 250;
        int thickness = 50;
        int pipeLength = 100;

        double expected = 110;
        double actual = _calculator.CalculateFirstLayerArea(diameter, thickness, pipeLength);

        Assert.Equal(expected, Math.Ceiling(actual));
    }

    [Fact]
    public void CalculateSecondLayerArea_ShouldReturnCorrectValue()
    {
        int diameter = 250;
        int thickness = 50;
        int pipeLength = 100;

        double expected = 142;

        double actual = _calculator.CalculateSecondLayerArea(diameter, thickness, pipeLength);

        Assert.Equal(expected, Math.Ceiling(actual));

    }

    [Fact]
    public void CalculateTotalArea_ShouldReturnSumOfLayers()
    {
        double firstLayer = 78.5;
        double secondLayer = 94.2;

        double expected = firstLayer + secondLayer;

        double actual = _calculator.CalculateTotalArea(firstLayer, secondLayer);

        Assert.Equal(Math.Ceiling(expected), Math.Ceiling(actual));
    }

    [Fact]
    public void CalculateRolls_ShouldReturnCorrectRollAmount()
    {
        double area = 100;
        double rollArea = 2.7;

        double expected = area / rollArea;

        double actual = _calculator.CalculateRolls(area, rollArea);

        Assert.Equal(Math.Ceiling(expected), Math.Ceiling(actual));
    }

    [Fact]
    public void CalculateTotalRolls_ShouldReturnSumOfRolls()
    {
        double rollsFirstLayer = 10;
        double rollsSecondLayer = 12;

        double expected = rollsFirstLayer + rollsSecondLayer;

        double actual = _calculator.CalculateTotalRolls(rollsFirstLayer, rollsSecondLayer);

        Assert.Equal(Math.Ceiling(expected), Math.Ceiling(actual));
    }

}