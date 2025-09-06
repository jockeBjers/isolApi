namespace IsolkalkylAPI.Controllers.Projects;

public class ProjectService(IDatabase DbContext) : IProjectService
{
    private readonly IDatabase _db = DbContext;

    public async Task<Project?> GetProjectByProjectNumber(string ProjectNumber)
    {

        var project = await _db.Projects
            .Include(p => p.Organization)
            .Include(p => p.Pipes)
                .ThenInclude(pipe => pipe.FirstLayerMaterial)
            .Include(p => p.Pipes)
                .ThenInclude(pipe => pipe.SecondLayerMaterial)
            .FirstOrDefaultAsync(p => p.ProjectNumber == ProjectNumber);

        Log.Information($"Project loaded: {project?.Name}, Pipe count: {project?.Pipes?.Count ?? 0}");
        return project;
    }
    public Task AddProject(Project project)
    {
        _db.Projects.Add(project);
        return _db.SaveChangesAsync();
    }

    public Task<bool> DoesProjectExist(string projectNumber)
    {
        return _db.Projects.AnyAsync(p => p.ProjectNumber == projectNumber);
    }

    public async Task<bool> RemoveProjectById(string projectNumber)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.ProjectNumber == projectNumber);
        if (project == null) return false;
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Project?> UpdateProject(string projectNumber, Project updatedProject)
    {
        var existingProject = await _db.Projects
            .FirstOrDefaultAsync(p => p.ProjectNumber == projectNumber);
        if (existingProject == null) return null;

        existingProject.ProjectNumber = updatedProject.ProjectNumber;
        existingProject.Name = updatedProject.Name;
        existingProject.FromDate = updatedProject.FromDate;
        existingProject.ToDate = updatedProject.ToDate;
        existingProject.Address = updatedProject.Address;
        existingProject.Customer = updatedProject.Customer;
        existingProject.ContactPerson = updatedProject.ContactPerson;
        existingProject.ContactNumber = updatedProject.ContactNumber;
        existingProject.Comment = updatedProject.Comment;

        await _db.SaveChangesAsync();
        return existingProject;

    }
}