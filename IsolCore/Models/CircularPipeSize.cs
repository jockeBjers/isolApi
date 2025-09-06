public class CircularPipeSize
{
    public int Id { get; set; }
    public string Label { get; set; }
    public double Diameter { get; set; }

    public CircularPipeSize() { }
    public CircularPipeSize(int id, string label, double diameter)
    {
        Id = id;
        Label = label;
        Diameter = diameter;
    }
    public static readonly List<CircularPipeSize> StandardSizes = new List<CircularPipeSize>
    {
        new CircularPipeSize(1, "100 mm", 0.1),
        new CircularPipeSize(2, "125 mm", 0.125),
        new CircularPipeSize(3, "160 mm", 0.160),
        new CircularPipeSize(4, "200 mm", 0.2),
        new CircularPipeSize(5, "250 mm", 0.25),
        new CircularPipeSize(6, "315 mm", 0.315),
        new CircularPipeSize(7, "400 mm", 0.4),
        new CircularPipeSize(8, "500 mm", 0.5),
        new CircularPipeSize(9, "630 mm", 0.63),
        new CircularPipeSize(10, "800 mm", 0.8),
        new CircularPipeSize(11, "1000 mm", 1.0),
    };
}