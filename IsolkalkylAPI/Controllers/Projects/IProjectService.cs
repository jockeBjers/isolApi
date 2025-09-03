namespace IsolkalkylAPI.Controllers.Projects;

public interface IProjectService
{
    Task<Project?> GetProjectByProjectNumber(string projectNumber);
    Task<bool> DoesProjectExist(string projectNumber);
    Task AddProject(Project project);
    Task<bool> RemoveProjectById(string projectNumber);
    Task<Project?> UpdateProject(string projectNumber, Project updatedProject);
}