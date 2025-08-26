namespace IsolkalkylAPI.Controllers.Users;

public class UserValidator : AbstractValidator<CreateUserRequest>
{
    public UserValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name is required")
           .Length(2, 100).WithMessage("Name must be between 2 and 100 characters")
           .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("Name contains invalid characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email is too long");

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Organization ID is required")
            .Length(1, 50).WithMessage("Organization ID must be between 1 and 50 characters");

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => new[] { "Admin", "User", "Manager" }.Contains(role))
            .WithMessage("Role must be Admin, User, or Manager");

        RuleFor(x => x.InitialPassword)
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .When(x => !string.IsNullOrEmpty(x.InitialPassword));
    }
}

public class UserUpdateValidator : AbstractValidator<UpdateUserRequest>
{
    public UserUpdateValidator()
    {
        RuleFor(x => x.Name)
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("Name contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email is too long")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Role)
            .Must(role => new[] { "Admin", "User", "Manager" }.Contains(role))
            .WithMessage("Role must be Admin, User, or Manager")
            .When(x => !string.IsNullOrEmpty(x.Role));
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Name) ||
                      !string.IsNullOrEmpty(x.Email) ||
                      !string.IsNullOrEmpty(x.Phone) ||
                      !string.IsNullOrEmpty(x.Role))
            .WithMessage("At least one field must be provided for update");

    }
}

public class GetUserByEmailValidator : AbstractValidator<GetUserByEmailRequest>
{
    public GetUserByEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}

public class AdminUserUpdateValidator : AbstractValidator<AdminUpdateUserRequest>
{
    public AdminUserUpdateValidator()
    {
        RuleFor(x => x.Name)
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("Name contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email is too long")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Role)
            .Must(role => new[] { "Admin", "User", "Manager" }.Contains(role))
            .WithMessage("Role must be Admin, User, or Manager")
            .When(x => !string.IsNullOrEmpty(x.Role));

        RuleFor(x => x.OrganizationId)
            .Length(1, 50).WithMessage("Organization ID must be between 1 and 50 characters")
            .When(x => !string.IsNullOrEmpty(x.OrganizationId));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Name) ||
                      !string.IsNullOrEmpty(x.Email) ||
                      !string.IsNullOrEmpty(x.Phone) ||
                      !string.IsNullOrEmpty(x.Role) ||
                      !string.IsNullOrEmpty(x.OrganizationId))
            .WithMessage("At least one field must be provided for update");
    }
}

public class PasswordResetValidator : AbstractValidator<PasswordResetRequest>
{
    public PasswordResetValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}
