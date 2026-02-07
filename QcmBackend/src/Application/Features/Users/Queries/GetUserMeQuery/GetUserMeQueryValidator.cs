using FluentValidation;
using QcmBackend.Application.Common.Validation;
using QcmBackend.Application.Features.Users.Queries.GetUserMeQuery;

namespace QcmBackend.Application.Features.Users.Queries.GetUserMeQuery;

public class GetUserMeQueryValidator : AbstractValidator<GetUserMeQuery>
{
    public GetUserMeQueryValidator()
    {
        _ = RuleFor(x => x).NotNull().WithErrorCode(ValidationCodes.Required);
    }
}
