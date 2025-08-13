public class Project
{

    public int Id { get; set; }
    public string ProjectNumber { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string? OrganizationId { get; set; }
    public string? Address { get; set; }
    public string? Customer { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactNumber { get; set; }
    public string? Comment { get; set; }
}
