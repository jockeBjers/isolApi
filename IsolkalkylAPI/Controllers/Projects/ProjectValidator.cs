namespace IsolkalkylAPI.Controllers.Projects;

public class ProjectValidator : AbstractValidator<CreateProjectRequest>
{
    public ProjectValidator()
    {
        RuleFor(x => x.ProjectNumber).ValidateProjectNumber(isRequired: true);
        RuleFor(x => x.Name).ValidateName(isRequired: true);
        RuleFor(x => x.FromDate).LessThan(x => x.ToDate).WithMessage("FromDate must be earlier than ToDate");
        RuleFor(x => x.OrganizationId).ValidateOrganizationId(isRequired: true);
        RuleFor(x => x.Address).ValidateAddress(isRequired: false);
        RuleFor(x => x.Customer).ValidateName(isRequired: false);
        RuleFor(x => x.ContactPerson).ValidateName(isRequired: false);
        RuleFor(x => x.ContactNumber).ValidatePhone().When(x => !string.IsNullOrEmpty(x.ContactNumber));
        RuleFor(x => x.Comment).ValidateComment().When(x => !string.IsNullOrEmpty(x.Comment));
    }
}

public class ProjectUpdateValidator : AbstractValidator<UpdateProjectRequest>
{
    public ProjectUpdateValidator()
    {
        RuleFor(x => x.ProjectNumber).ValidateProjectNumber(isRequired: false);
        RuleFor(x => x.Name).ValidateName(isRequired: false);
        RuleFor(x => x.FromDate).LessThan(x => x.ToDate).WithMessage("FromDate must be earlier than ToDate");
        RuleFor(x => x.OrganizationId).ValidateOrganizationId(isRequired: false);
        RuleFor(x => x.Address).ValidateAddress(isRequired: false);
        RuleFor(x => x.Customer).ValidateName(isRequired: false);
        RuleFor(x => x.ContactPerson).ValidateName(isRequired: false);
        RuleFor(x => x.ContactNumber).ValidatePhone().When(x => !string.IsNullOrEmpty(x.ContactNumber));
        RuleFor(x => x.Comment).ValidateComment().When(x => !string.IsNullOrEmpty(x.Comment));

        RuleFor(x => x)
         .Must(x => !string.IsNullOrEmpty(x.ProjectNumber)
                 || !string.IsNullOrEmpty(x.Name)
                 || x.FromDate.HasValue
                 || x.ToDate.HasValue
                 || !string.IsNullOrEmpty(x.OrganizationId)
                 || !string.IsNullOrEmpty(x.Address)
                 || !string.IsNullOrEmpty(x.Customer)
                 || !string.IsNullOrEmpty(x.ContactPerson)
                 || !string.IsNullOrEmpty(x.ContactNumber)
                 || !string.IsNullOrEmpty(x.Comment))
         .WithMessage("At least one field must be provided for update");
    }
}