using MediatR;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Auth.Dtos;

namespace QcmBackend.Application.Features.Auth.Commands.LoginCommand
{
    public class LoginCommand : IRequest<Result<ReadTokenDto>>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required bool RememberMe { get; init; }
    }
}
