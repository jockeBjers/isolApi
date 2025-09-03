namespace IsolkalkylAPI.Controllers.Projects;

public interface IProjectService
{
    Task<Project?> GetProjectByProjectNumber(string projectNumber);
    Task<bool> DoesProjectExist(string projectNumber);
    Task AddProject(Project project);
    Task<bool> RemoveProjectById(int projectId);
    Task<Project?> UpdateProject(int projectId, Project updatedProject);
}