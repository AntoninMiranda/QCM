using QcmBackend.Application.Common.Validation;
using FluentValidation;

namespace QcmBackend.Application.Features.Auth.Commands.RegisterCommand;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        _ = RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ValidationCodes.Required)
            .EmailAddress().WithErrorCode(ValidationCodes.Invalid);

        _ = RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(ValidationCodes.Required)
            .MinimumLength(8).WithErrorCode(ValidationCodes.TooShort)
            .MaximumLength(100).WithErrorCode(ValidationCodes.TooLong)
            .Matches(@"\d").WithErrorCode(ValidationCodes.Invalid);

        _ = RuleFor(x => x.FirstName)
            .NotEmpty().WithErrorCode(ValidationCodes.Required)
            .MinimumLength(2).WithErrorCode(ValidationCodes.TooShort)
            .MaximumLength(100).WithErrorCode(ValidationCodes.TooLong)
            .Matches(@"^[\p{L}'-]+( [\p{L}'-]+)*$").WithErrorCode(ValidationCodes.Invalid);

        _ = RuleFor(x => x.LastName)
            .NotEmpty().WithErrorCode(ValidationCodes.Required)
            .MinimumLength(2).WithErrorCode(ValidationCodes.TooShort)
            .MaximumLength(100).WithErrorCode(ValidationCodes.TooLong)
            .Matches(@"^[\p{L}'-]+( [\p{L}'-]+)*$").WithErrorCode(ValidationCodes.Invalid);
    }
}
