namespace IsolkalkylAPI.Controllers.Auth;
public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).ValidateEmail(isRequired: true);

        RuleFor(x => x.Password).ValidatePassword(isRequired: true);
    }
}

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name).ValidateName(isRequired: true);
        RuleFor(x => x.Password).ValidatePassword(isRequired: true);
        RuleFor(x => x.Email).ValidateEmail(isRequired: true);
        RuleFor(x => x.OrganizationId).ValidateOrganizationId(isRequired: true);
        RuleFor(x => x.PhoneNumber).ValidatePhone();
    }
}
