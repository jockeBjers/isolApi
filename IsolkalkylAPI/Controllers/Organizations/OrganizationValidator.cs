namespace IsolkalkylAPI.Controllers.Organizations;

public class OrganizationValidator : AbstractValidator<CreateOrganizationRequest>
{
    public OrganizationValidator()
    {
        RuleFor(x => x.Id).ValidateId(isRequired: true);
        RuleFor(x => x.Name).ValidateName(isRequired: true);
        RuleFor(x => x.Address).ValidateAddress(isRequired: true);
        RuleFor(x => x.Phone).ValidatePhone();
        RuleFor(x => x.Email).ValidateEmail(isRequired: true);
        RuleFor(x => x.Website).ValidateWebsite(isRequired: true);
    }
}

public class OrganizationUpdateValidator : AbstractValidator<UpdateOrganizationRequest>
{
    public OrganizationUpdateValidator()
    {
        RuleFor(x => x.Name).ValidateName(isRequired: false);
        RuleFor(x => x.Address).ValidateAddress(isRequired: false);
        RuleFor(x => x.Phone).ValidatePhone();
        RuleFor(x => x.Email).ValidateEmail(isRequired: false);
        RuleFor(x => x.Website).ValidateWebsite(isRequired: false);
        
        RuleFor(x => x)
         .Must(x => !string.IsNullOrEmpty(x.Name)
                 || !string.IsNullOrEmpty(x.Address)
                 || !string.IsNullOrEmpty(x.Phone)
                 || !string.IsNullOrEmpty(x.Email)
                 || !string.IsNullOrEmpty(x.Website))
         .WithMessage("At least one field must be provided for update");

    }
}
