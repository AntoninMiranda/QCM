using FluentValidation;
using QcmBackend.Application.Common.Validation;

namespace QcmBackend.Application.Features.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        _ = RuleFor(x => x).NotNull().WithErrorCode(ValidationCodes.Required);
    }
}