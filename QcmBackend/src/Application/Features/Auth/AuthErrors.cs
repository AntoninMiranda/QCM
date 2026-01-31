using QcmBackend.Application.Common.Result;

namespace QcmBackend.Application.Features.Auth
{
    public class AuthErrors : EntityError<AuthErrors>
    {
        protected override string Entity => "Auth";
    }
}
