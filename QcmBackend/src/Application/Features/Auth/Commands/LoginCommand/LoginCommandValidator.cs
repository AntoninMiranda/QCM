using FluentValidation;
using QcmBackend.Application.Common.Validation;

namespace QcmBackend.Application.Features.Auth.Commands.LoginCommand;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        _ = RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ValidationCodes.Required)
            .EmailAddress().WithErrorCode(ValidationCodes.Invalid);

        _ = RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(ValidationCodes.Required);
    }
}