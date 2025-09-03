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
);

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
);
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