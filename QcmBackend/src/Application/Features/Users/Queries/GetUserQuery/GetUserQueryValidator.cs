using FluentValidation;
using QcmBackend.Application.Common.Validation;

namespace QcmBackend.Application.Features.Users.Queries.GetUserQuery;

public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty().WithErrorCode(ValidationCodes.Required);
    }
}