namespace IsolkalkylAPI.Controllers.Projects;

public record ProjectResponse(
    int Id,
    string ProjectNumber,
    string Name,
    DateTime FromDate,
    DateTime ToDate,
    string OrganizationId,
    string? Address,
    string? Customer,
    string? ContactPerson,
    string? ContactNumber,
    string? Comment
// TODO: Add pipes later: List<InsulatedPipeDto> Pipes
)
{
    public static ProjectResponse FromProject(Project project)
    {
        return new ProjectResponse(
            project.Id,
            project.ProjectNumber,
            project.Name,
            project.FromDate,
            project.ToDate,
            project.OrganizationId,
            project.Address,
            project.Customer,
            project.ContactPerson,
            project.ContactNumber,
            project.Comment
        );
    }
}
public record ProjectListResponse(
    int Id,
    string ProjectNumber,
    string Name,
    DateTime FromDate,
    DateTime ToDate
);

public record CreateProjectRequest(
    string ProjectNumber,
    string Name,
    DateTime FromDate,
    DateTime ToDate,
    string OrganizationId,
    string? Address,
    string? Customer,
    string? ContactPerson,
    string? ContactNumber,
    string? Comment
)
{
    public Project ToProject()
    {
        return new Project
        {
            ProjectNumber = ProjectNumber,
            Name = Name,
            OrganizationId = OrganizationId,
            FromDate = FromDate,
            ToDate = ToDate,
            Address = Address,
            Customer = Customer,
            ContactPerson = ContactPerson,
            ContactNumber = ContactNumber,
            Comment = Comment
        };
    }
}
public record UpdateProjectRequest(
    string? ProjectNumber,
    string? Name,
    DateTime? FromDate,
    DateTime? ToDate,
    string? OrganizationId,
    string? Address,
    string? Customer,
    string? ContactPerson,
    string? ContactNumber,
    string? Comment
);

public static class ProjectRequestExtensions
{
    public static void ApplyTo(this UpdateProjectRequest request, Project existingProject)
    {
        if (!string.IsNullOrEmpty(request.ProjectNumber))
            existingProject.ProjectNumber = request.ProjectNumber;
        if (!string.IsNullOrEmpty(request.Name))
            existingProject.Name = request.Name;
        if (request.FromDate.HasValue)
            existingProject.FromDate = request.FromDate.Value;
        if (request.ToDate.HasValue)
            existingProject.ToDate = request.ToDate.Value;
        if (!string.IsNullOrEmpty(request.OrganizationId))
            existingProject.OrganizationId = request.OrganizationId;
        if (!string.IsNullOrEmpty(request.Address))
            existingProject.Address = request.Address;
        if (!string.IsNullOrEmpty(request.Customer))
            existingProject.Customer = request.Customer;
        if (!string.IsNullOrEmpty(request.ContactPerson))
            existingProject.ContactPerson = request.ContactPerson;
        if (!string.IsNullOrEmpty(request.ContactNumber))
            existingProject.ContactNumber = request.ContactNumber;
        if (!string.IsNullOrEmpty(request.Comment))
            existingProject.Comment = request.Comment;
    }
}