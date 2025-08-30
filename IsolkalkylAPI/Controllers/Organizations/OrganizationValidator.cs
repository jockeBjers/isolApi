using IsolkalkylAPI.Controllers.Validators;
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
        RuleFor(x => x.Name).ValidateName(isRequired: true);
        RuleFor(x => x.Address).ValidateAddress(isRequired: true);
        RuleFor(x => x.Phone).ValidatePhone();
        RuleFor(x => x.Email).ValidateEmail(isRequired: true);
        RuleFor(x => x.Website).ValidateWebsite(isRequired: true);
    }
}
