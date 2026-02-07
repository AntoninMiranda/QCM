using QcmBackend.Application.Common.Result;

namespace QcmBackend.Application.Features.Users;

public class UsersErrors : EntityError<UsersErrors>
{
    protected override string Entity => "User";

    public static Error InvalidCredentials()
    {
        return new Error(
            code: $"{Instance.Entity}.InvalidCredentials",
            message: "Invalid email or password.",
            errorType: ErrorType.AccessUnAuthorized
        );
    }

    public static Error InvalidToken()
    {
        return new Error(
            code: $"{Instance.Entity}.InvalidToken",
            message: "Invalid token.",
            errorType: ErrorType.Validation
        );
    }
}
    