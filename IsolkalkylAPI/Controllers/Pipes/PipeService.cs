
namespace IsolkalkylAPI.Controllers.Pipes;

public class PipesService(IDatabase DbContext) : IPipeService
{
    private readonly IDatabase _db = DbContext;

    public async Task<List<InsulatedPipeBase>> GetAllPipesInProject(string projectNumber)
    {
        return await _db.Pipes.Where(p => p.ProjectNumber == projectNumber).ToListAsync();
    }
    public async Task<InsulatedPipeBase> AddPipe(InsulatedPipeBase pipe)
    {
        await _db.Pipes.AddAsync(pipe);
        await _db.SaveChangesAsync();
        
        return await _db.Pipes
            .Include(p => p.FirstLayerMaterial)
            .Include(p => p.SecondLayerMaterial)
            .Include(p => p.Project)
            .FirstAsync(p => p.Id == pipe.Id);
    }

    public async Task<bool> DoesPipeExist(int pipeId)
    {
        return await _db.Pipes.AnyAsync(p => p.Id == pipeId);
    }
    public async Task<bool> RemovePipeById(int pipeId)
    {
        var pipe = await GetPipeById(pipeId);
        if (pipe == null) return false;
        _db.Pipes.Remove(pipe);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<InsulatedPipeBase?> UpdatePipe(int pipeId, InsulatedPipeBase updatedPipe)
    {
        var pipe = await GetPipeById(pipeId);
        if (pipe == null) return null;

        if (pipe is CircularInsulatedPipe circularPipe && updatedPipe is CircularInsulatedPipe updatedCircularPipe)
        {
            circularPipe.Size = updatedCircularPipe.Size;
            circularPipe.Length = updatedCircularPipe.Length;
            circularPipe.FirstLayerMaterial = updatedCircularPipe.FirstLayerMaterial;
            circularPipe.SecondLayerMaterial = updatedCircularPipe.SecondLayerMaterial;
            circularPipe.ProjectNumber = updatedCircularPipe.ProjectNumber;
        }
        else if (pipe is RectangularInsulatedPipe rectangularPipe && updatedPipe is RectangularInsulatedPipe updatedRectangularPipe)
        {
            rectangularPipe.SideA = updatedRectangularPipe.SideA;
            rectangularPipe.SideB = updatedRectangularPipe.SideB;
            rectangularPipe.Length = updatedRectangularPipe.Length;
            rectangularPipe.FirstLayerMaterial = updatedRectangularPipe.FirstLayerMaterial;
            rectangularPipe.SecondLayerMaterial = updatedRectangularPipe.SecondLayerMaterial;
            rectangularPipe.ProjectNumber = updatedRectangularPipe.ProjectNumber;
        }
        else
        {
            return null;
        }

        await _db.SaveChangesAsync();
        return pipe;
    }
    public async Task<InsulatedPipeBase?> GetPipeById(int pipeId)
    {
        return await _db.Pipes
          .Include(p => p.FirstLayerMaterial)
          .Include(p => p.SecondLayerMaterial)
          .Include(p => p.Project)
          .FirstOrDefaultAsync(p => p.Id == pipeId);
    }


    public async Task<List<InsulatedPipeBase>> GetPipesByTypeInProject(string projectNumber, string pipeType)
    {
        return await _db.Pipes
            .Where(p => p.ProjectNumber == projectNumber && p.GetType().Name.Equals(pipeType, StringComparison.OrdinalIgnoreCase))
            .ToListAsync();
    }

    /* CALCULATIONS */
    public async Task<double> GetFirstLayerAreaForPipe(int pipeId)
    {
        var pipe = await GetPipeById(pipeId);
        if (pipe == null) return 0;
        return pipe.GetFirstLayerArea();
    }

    public async Task<double> GetSecondLayerAreaForPipe(int pipeId)
    {
        var pipe = await GetPipeById(pipeId);
        if (pipe == null) return 0;
        return pipe.GetSecondLayerArea();
    }

    public async Task<double> GetTotalAreaForPipe(int pipeId)
    {
        var pipe = await GetPipeById(pipeId);
        if (pipe == null) return 0;
        return pipe.GetTotalArea();
    }

    public async Task<Dictionary<string, double>> GetAreaByInsulationType(string projectNumber)
    {
        var materialUsage = new Dictionary<string, double>();

        var insulationTypes = await _db.InsulationTypes.ToListAsync();

        var projectPipes = await _db.Pipes
            .Where(p => p.ProjectNumber == projectNumber)
            .Include(p => p.FirstLayerMaterial)
            .Include(p => p.SecondLayerMaterial)
            .ToListAsync();

        // calculate area for each insulation type
        foreach (var insulationType in insulationTypes)
        {
            var firstLayerArea = projectPipes
                .Where(p => p.FirstLayerMaterial?.Id == insulationType.Id)
                .Sum(p => p.GetFirstLayerArea());

            var secondLayerArea = projectPipes
                .Where(p => p.SecondLayerMaterial?.Id == insulationType.Id)
                .Sum(p => p.GetSecondLayerArea());

            var totalArea = firstLayerArea + secondLayerArea;

            if (totalArea > 0)
            {
                materialUsage[insulationType.Name] = totalArea;
            }
        }

        return materialUsage;
    }
}
