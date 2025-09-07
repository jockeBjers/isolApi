namespace IsolkalkylAPI.Controllers.Validators;

public static class UserRoles
{
    public static readonly string[] ValidRoles = { "Admin", "User", "Manager" };
}
public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> ValidateEmail<T>(this IRuleBuilder<T, string?> rule, bool isRequired = true)
    {
        var builder = rule.EmailAddress().WithMessage("Invalid email format")
                         .MaximumLength(50).WithMessage("Email must be at most 50 characters.");

        if (isRequired)
            builder = builder.NotEmpty().WithMessage("Email is required")
            .NotNull().WithMessage("Email is required");

        return builder;
    }
    public static IRuleBuilderOptions<T, string?> ValidateId<T>(this IRuleBuilder<T, string?> rule, bool isRequired = true)
    {
        var builder = rule.Length(1, 50).WithMessage("ID must be between 1 and 50 characters");

        if (isRequired)
            builder = builder.NotEmpty().WithMessage("ID is required");

        return builder;
    }

    public static IRuleBuilderOptions<T, string?> ValidateAddress<T>(this IRuleBuilder<T, string?> rule, bool isRequired = true)
    {
        var builder = rule.MaximumLength(200).WithMessage("Address must not be more than 200 characters");

        if (isRequired)
            builder = builder.NotEmpty().WithMessage("Address is required");

        return builder;
    }

    public static IRuleBuilderOptions<T, string?> ValidateWebsite<T>(this IRuleBuilder<T, string?> rule, bool isRequired = true)
    {
        var builder = rule.Must(uri =>
                string.IsNullOrEmpty(uri) ||
                Uri.IsWellFormedUriString(uri, UriKind.Absolute) ||
                Uri.IsWellFormedUriString("https://" + uri, UriKind.Absolute)
            )
            .WithMessage("Invalid website URL format");

        if (isRequired)
            builder = builder.NotEmpty().WithMessage("Website is required");

        return builder;
    }

    public static IRuleBuilderOptions<T, string?> ValidateName<T>(this IRuleBuilder<T, string?> rule, bool isRequired = true)
    {
        var builder = rule.Length(2, 100).WithMessage("Name must be between 2 and 100 characters")
                         .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("Name contains invalid characters");

        if (isRequired)
            builder = builder.NotEmpty().WithMessage("Name is required");

        return builder;
    }

    public static IRuleBuilderOptions<T, string?> ValidatePhone<T>(this IRuleBuilder<T, string?> rule)
    {
        return rule.Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format");
    }

    public static IRuleBuilderOptions<T, string?> ValidateRole<T>(this IRuleBuilder<T, string?> rule, bool isRequired = true)
    {
        var builder = rule.Must(role => role == null || UserRoles.ValidRoles.Contains(role))
                         .WithMessage($"Role must be one of: {string.Join(", ", UserRoles.ValidRoles)}");

        if (isRequired)
            builder = builder.NotEmpty().WithMessage("Role is required");

        return builder;
    }

    public static IRuleBuilderOptions<T, string?> ValidateOrganizationId<T>(this IRuleBuilder<T, string?> rule, bool isRequired = true)
    {
        var builder = rule.Length(1, 50).WithMessage("Organization ID must be between 1 and 50 characters");

        if (isRequired)
            builder = builder.NotEmpty().WithMessage("Organization ID is required");

        return builder;
    }

    public static IRuleBuilderOptions<T, string?> ValidateComment<T>(this IRuleBuilder<T, string?> rule)
    {
        return rule.MaximumLength(500).WithMessage("Comment must not be more than 500 characters");
    }


    public static IRuleBuilderOptions<T, string?> ValidateProjectNumber<T>(this IRuleBuilder<T, string?> rule, bool isRequired = true)
    {
        var builder = rule.Length(1, 10).WithMessage("Project Number must be between 1 and 10 characters")
                         .Matches(@"^\d+$").WithMessage("Project Number must contain only digits");

        if (isRequired)
            builder = builder.NotEmpty().WithMessage("Project Number is required");
        return builder;
    }
    public static IRuleBuilderOptions<T, string?> ValidatePassword<T>(this IRuleBuilder<T, string?> rule, bool isRequired = true)
    {
        var builder = rule.MinimumLength(8).WithMessage("Password must be at least 8 characters")
                         .MaximumLength(100).WithMessage("Password must be at most 100 characters")
                         .Must(HasUppercaseLetter).WithMessage("Password must contain at least one uppercase letter")
                         .Must(HasLowercaseLetter).WithMessage("Password must contain at least one lowercase letter")
                         .Must(HasDigit).WithMessage("Password must contain at least one digit")
                         .Must(HasSpecialCharacter).WithMessage("Password must contain at least one special character");

        if (isRequired)
            builder = builder.NotEmpty().WithMessage("Password is required")
            .NotNull().WithMessage("Password is required");

        return builder;
    }

    public static IRuleBuilderOptions<T, string?> ValidatePipeType<T>(this IRuleBuilder<T, string?> rule, bool isRequired = true)
    {
        var builder = rule.Must(type => type == null || type.Equals("Circular", StringComparison.OrdinalIgnoreCase) || type.Equals("Rectangular", StringComparison.OrdinalIgnoreCase))
                         .WithMessage("Pipe Type must be either 'Circular' or 'Rectangular'");

        if (isRequired)
            builder = builder.NotEmpty().WithMessage("Pipe Type is required")
                             .NotNull().WithMessage("Pipe Type is required");

        return builder;
    }

    public static IRuleBuilderOptions<T, double?> SideLengthValidation<T>(this IRuleBuilder<T, double?> rule, string name = "Side", bool isRequired = true)
    {
        var builder = rule
            .InclusiveBetween(0, 4000)
            .WithMessage($"{name} must be between 0 and 4000");

        if (isRequired)
        {
            builder = builder
                .NotNull()
                .WithMessage($"{name} is required");
        }

        return builder;
    }


    private static bool HasUppercaseLetter(string? password) => password?.Any(char.IsUpper) ?? false;
    private static bool HasLowercaseLetter(string? password) => password?.Any(char.IsLower) ?? false;
    private static bool HasDigit(string? password) => password?.Any(char.IsDigit) ?? false;
    private static bool HasSpecialCharacter(string? password) => password?.Any(ch => !char.IsLetterOrDigit(ch)) ?? false;
}