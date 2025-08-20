namespace IsolkalkylAPI.Controllers.Auth;

using FluentValidation;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .NotNull().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(40).WithMessage("Email must be at most 40 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .NotNull().WithMessage("Password is required.")
            .MaximumLength(40).WithMessage("Password must be at most 40 characters.");
    }
}

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .NotNull().WithMessage("Email is required.")
            .MinimumLength(4).WithMessage("Email must be at least 4 characters.")
            .MaximumLength(40).WithMessage("Email must be at most 40 characters.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .NotNull().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(100).WithMessage("Password must be at most 100 characters.")
            .Must(HasUppercaseLetter).WithMessage("Password must contain at least one uppercase letter.")
            .Must(HasLowercaseLetter).WithMessage("Password must contain at least one lowercase letter.")
            .Must(HasDigit).WithMessage("Password must contain at least one digit.")
            .Must(HasSpecialCharacter).WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.Email)
            .NotEmpty().NotNull()
            .EmailAddress()
            .MaximumLength(60);
    }
    private bool HasUppercaseLetter(string password)
    {
        return password.Any(char.IsUpper);
    }

    private bool HasLowercaseLetter(string password)
    {
        return password.Any(char.IsLower);
    }

    private bool HasDigit(string password)
    {
        return password.Any(char.IsDigit);
    } 

    private bool HasSpecialCharacter(string password)
    {
        return password.Any(c => !char.IsLetterOrDigit(c));
    }
}
