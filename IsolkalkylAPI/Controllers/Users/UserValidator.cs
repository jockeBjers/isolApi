using IsolkalkylAPI.Controllers.Validators;
namespace IsolkalkylAPI.Controllers.Users;

public class UserValidator : AbstractValidator<CreateUserRequest>
{
    public UserValidator()
    {
        RuleFor(x => x.Name).ValidateName(isRequired: true);
        RuleFor(x => x.Email).ValidateEmail(isRequired: true);
        RuleFor(x => x.OrganizationId).ValidateOrganizationId(isRequired: true);
        RuleFor(x => x.Phone).ValidatePhone().When(x => !string.IsNullOrEmpty(x.Phone));
        RuleFor(x => x.Role).ValidateRole(isRequired: true);
        RuleFor(x => x.Password).ValidatePassword(isRequired: true);
    }
}

public class UserUpdateValidator : AbstractValidator<UpdateUserRequest>
{
    public UserUpdateValidator()
    {
        RuleFor(x => x.Name).ValidateName(isRequired: false);
        RuleFor(x => x.Email).ValidateEmail(isRequired: false);
        RuleFor(x => x.Phone).ValidatePhone().When(x => !string.IsNullOrEmpty(x.Phone));
        RuleFor(x => x.Role).ValidateRole(isRequired: false);

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.Email) ||
                      !string.IsNullOrEmpty(x.Phone) || !string.IsNullOrEmpty(x.Role))
            .WithMessage("At least one field must be provided for update");
    }
}

public class GetUserByEmailValidator : AbstractValidator<GetUserByEmailRequest>
{
    public GetUserByEmailValidator()
    {
        RuleFor(x => x.Email).ValidateEmail(isRequired: true);
    }
}

public class AdminUserUpdateValidator : AbstractValidator<AdminUpdateUserRequest>
{
    public AdminUserUpdateValidator()
    {
        RuleFor(x => x.Name).ValidateName(isRequired: false);
        RuleFor(x => x.Email).ValidateEmail(isRequired: false);
        RuleFor(x => x.Phone).ValidatePhone().When(x => !string.IsNullOrEmpty(x.Phone));
        RuleFor(x => x.Role).ValidateRole(isRequired: false);
        RuleFor(x => x.OrganizationId).ValidateOrganizationId(isRequired: false).When(x => !string.IsNullOrEmpty(x.OrganizationId));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.Email) ||
                      !string.IsNullOrEmpty(x.Phone) || !string.IsNullOrEmpty(x.Role) ||
                      !string.IsNullOrEmpty(x.OrganizationId))
            .WithMessage("At least one field must be provided for update");
    }
}

public class PasswordResetValidator : AbstractValidator<PasswordResetRequest>
{
    public PasswordResetValidator()
    {
        RuleFor(x => x.Email).ValidateEmail(isRequired: true);
    }
}