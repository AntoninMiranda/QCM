using QcmBackend.Application.Common.Validation;
using FluentValidation;

namespace QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand;

public class UpdateUserMeCommandValidator : AbstractValidator<UpdateUserMeCommand>
{
    public UpdateUserMeCommandValidator()
    {
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
