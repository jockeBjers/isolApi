namespace IsolkalkylAPI.Controllers.Pipes;

public interface IPipeService
{
    Task<List<InsulatedPipeBase>> GetAllPipesInProject(string projectNumber);
    Task<InsulatedPipeBase?> GetPipeById(int pipeId);
    Task<bool> DoesPipeExist(int pipeId);
    Task AddPipe(InsulatedPipeBase pipe);
    Task<bool> RemovePipeById(int pipeId);
    Task<InsulatedPipeBase?> UpdatePipe(int pipeId, InsulatedPipeBase updatedPipe);
    Task<List<InsulatedPipeBase>> GetPipesByTypeInProject(string projectNumber, string pipeType);

    /* calculations */
    Task<Dictionary<string, double>> GetAreaByInsulationType(string projectNumber);
    Task<double> GetFirstLayerAreaForPipe(int pipeId);
    Task<double> GetSecondLayerAreaForPipe(int pipeId);
    Task<double> GetTotalAreaForPipe(int pipeId);

}
