namespace IsolkalkylAPI.Controllers.Pipes;

public class PipeValidator : AbstractValidator<CreatePipeRequest>
{
    public PipeValidator()
    {
        RuleFor(x => x.ProjectNumber).ValidateProjectNumber(isRequired: true);
        RuleFor(x => x.PipeType).ValidatePipeType(isRequired: true);
        RuleFor(x => x.Length).GreaterThan(0).WithMessage("Length must be greater than 0");
        RuleFor(x => x.SideA).SideLengthValidation("SideA").When(x => x.PipeType == "Rectangular");
        RuleFor(x => x.SideB).SideLengthValidation("SideB").When(x => x.PipeType == "Rectangular");
        RuleFor(x => x.SizeId).NotNull().WithMessage("Size is required for Circular pipes").When(x => x.PipeType == "Circular");
        RuleFor(x => x.FirstLayerMaterialId).NotEmpty().WithMessage("FirstLayerMaterialId is required");
    }
}
public class PipeUpdateValidator : AbstractValidator<UpdatePipeRequest>
{
    public PipeUpdateValidator()
    {
        RuleFor(x => x.Length).GreaterThan(0).WithMessage("Length must be greater than 0")
            .When(x => x.Length.HasValue);
         RuleFor(x => x.SideA).SideLengthValidation("SideA", isRequired: false)
            .When(x => x.SideA.HasValue);
        RuleFor(x => x.SideB).SideLengthValidation("SideB", isRequired: false)
            .When(x => x.SideB.HasValue);
        RuleFor(x => x).Must(x => !(x.SideA.HasValue ^ x.SideB.HasValue))
            .WithMessage("Both SideA and SideB must be provided for a rectangular pipe");
        RuleFor(x => x.SizeId).NotNull().WithMessage("SizeId is required for circular pipes")
            .When(x => x.SizeId.HasValue);
        RuleFor(x => x.FirstLayerMaterialId).NotEmpty().WithMessage("FirstLayerMaterialId is required")
            .When(x => x.FirstLayerMaterialId.HasValue);
        RuleFor(x => x)
            .Must(x => x.SizeId.HasValue || (x.SideA.HasValue && x.SideB.HasValue))
            .WithMessage("You must provide either SizeId (circular) or both SideA and SideB (rectangular)")
            .When(x => x.SizeId.HasValue || x.SideA.HasValue || x.SideB.HasValue);

        RuleFor(x => x)
            .Must(x => x.Length.HasValue
                    || x.FirstLayerMaterialId.HasValue
                    || x.SecondLayerMaterialId.HasValue
                    || x.SizeId.HasValue
                    || x.SideA.HasValue
                    || x.SideB.HasValue)
            .WithMessage("At least one field must be provided for update");
    }
}