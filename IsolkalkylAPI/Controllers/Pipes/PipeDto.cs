
namespace IsolkalkylAPI.Controllers.Pipes;

public record PipeResponse(
    int Id,
    string ProjectNumber,
    double Length,
    InsulationTypeDto FirstLayerMaterial,
    InsulationTypeDto? SecondLayerMaterial,
    string PipeType,
    CircularPipeSizeDto? Size,
    double? SideA,
    double? SideB
)
{
    public static PipeResponse FromInsulatedPipe(InsulatedPipeBase pipe)
    {
        var id = pipe.Id;
        var projectNumber = pipe.ProjectNumber;
        var length = pipe.Length;
        var firstLayer = pipe.FirstLayerMaterial != null
        ? InsulationTypeDto.FromInsulationType(pipe.FirstLayerMaterial)
        : throw new InvalidOperationException("FirstLayerMaterial is required but was null");

        var secondLayer = pipe.SecondLayerMaterial != null
            ? InsulationTypeDto.FromInsulationType(pipe.SecondLayerMaterial)
            : null;

        PipeResponse response = pipe switch
        {
            CircularInsulatedPipe circularPipe => new PipeResponse(
                        id,
                        projectNumber,
                        length,
                        firstLayer,
                        secondLayer,
                        "Circular",
                        circularPipe.Size != null
                            ? CircularPipeSizeDto.FromCircularPipeSize(circularPipe.Size)
                            : null,
                        null,
                        null
                    ),
            RectangularInsulatedPipe rectangularPipe => new PipeResponse(
                        id,
                        projectNumber,
                        length,
                        firstLayer,
                        secondLayer,
                        "Rectangular",
                        null,
                        rectangularPipe.SideA,
                        rectangularPipe.SideB
                    ),
            _ => throw new InvalidOperationException($"Unknown pipe type: {pipe.GetType().Name}"),
        };
        return response;

    }
}

// Create request
public record CreatePipeRequest(
    string ProjectNumber,
    double Length,
    string PipeType,
    int FirstLayerMaterialId,
    int? SecondLayerMaterialId,
    // Circular pipe fields 
    int? SizeId,
    // Rectangular pipe fields 
    double? SideA,
    double? SideB
)
{
    public InsulatedPipeBase ToPipe()
    {
        return PipeType.ToLower() switch
        {
            "circular" => new CircularInsulatedPipe
            {
                ProjectNumber = ProjectNumber,
                Length = Length,
                FirstLayerMaterialId = FirstLayerMaterialId,
                SecondLayerMaterialId = SecondLayerMaterialId,
                SizeId = SizeId ?? throw new ArgumentException("SizeId is required for circular pipes")
            },
            "rectangular" => new RectangularInsulatedPipe
            {
                ProjectNumber = ProjectNumber,
                Length = Length,
                FirstLayerMaterialId = FirstLayerMaterialId,
                SecondLayerMaterialId = SecondLayerMaterialId,
                SideA = SideA ?? throw new ArgumentException("SideA is required for rectangular pipes"),
                SideB = SideB ?? throw new ArgumentException("SideB is required for rectangular pipes")
            },
            _ => throw new ArgumentException($"Invalid pipe type: {PipeType}. Must be 'Circular' or 'Rectangular'."),
        };
    }
}

// Update requests
public record UpdatePipeRequest(
    double? Length,
    int? FirstLayerMaterialId,
    int? SecondLayerMaterialId,
    // Circular pipe fieldsF
    int? SizeId,
    // Rectangular pipe fields
    double? SideA,
    double? SideB
);

public static class PipeRequestExtensions
{
    public static void ApplyTo(this UpdatePipeRequest request, InsulatedPipeBase existingPipe)
    {

        if (request.Length.HasValue)
            existingPipe.Length = request.Length.Value;
        if (request.FirstLayerMaterialId.HasValue)
            existingPipe.FirstLayerMaterialId = request.FirstLayerMaterialId.Value;
        if (request.SecondLayerMaterialId.HasValue)
            existingPipe.SecondLayerMaterialId = request.SecondLayerMaterialId.Value;

        switch (existingPipe)
        {
            case CircularInsulatedPipe circularPipe:
                if (request.SizeId.HasValue)
                    circularPipe.Size = new CircularPipeSize { Id = request.SizeId.Value };
                break;

            case RectangularInsulatedPipe rectangularPipe:
                if (request.SideA.HasValue)
                    rectangularPipe.SideA = request.SideA.Value;
                if (request.SideB.HasValue)
                    rectangularPipe.SideB = request.SideB.Value;
                break;

            default:
                throw new InvalidOperationException($"Unknown pipe type: {existingPipe.GetType().Name}");
        }
    }

}



// DTOs for related entities, TODO: move later
public record InsulationTypeDto(
    int Id,
    string Name,
    double InsulationThickness,
    double InsulationAreaPerMeter,
    string InsulationCategory,
    string OrganizationId
)
{
    public static InsulationTypeDto FromInsulationType(InsulationType insulationType)
    {
        return new InsulationTypeDto(
            insulationType.Id,
            insulationType.Name,
            insulationType.InsulationThickness,
            insulationType.InsulationAreaPerMeter,
            insulationType.InsulationCategory,
            insulationType.OrganizationId
        );
    }
}

public record CircularPipeSizeDto(
    int Id,
    string Label,
    double Diameter
)
{
    public static CircularPipeSizeDto FromCircularPipeSize(CircularPipeSize size)
    {
        return new CircularPipeSizeDto(
            size.Id,
            size.Label,
            size.Diameter
        );
    }
}