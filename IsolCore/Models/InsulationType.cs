public class InsulationType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double InsulationThickness { get; set; }
    public double InsulationAreaPerMeter { get; set; }
    public string InsulationCategory { get; set; }

    public InsulationType(int id, string name, double insulationThickness, double insulationAreaPerMeter, string insulationCategory = "heat")
    {
        Id = id;
        Name = name;
        InsulationThickness = insulationThickness;
        InsulationAreaPerMeter = insulationAreaPerMeter;
        InsulationCategory = insulationCategory;
    }

    public static List<InsulationType> GetDefaultTypes()
    {
        return new List<InsulationType>
        {
            new InsulationType(1, "30mm, 3.6m²", 0.03, 3.6, "heat"),
            new InsulationType(2, "50mm, 2.7m²", 0.05, 2.7, "heat"),
            new InsulationType(3, "80mm, 1.5m²", 0.08, 1.5, "heat")
        };
    }

}
