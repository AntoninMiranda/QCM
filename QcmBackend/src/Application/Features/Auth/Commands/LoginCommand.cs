using MediatR;
using QcmBackend.Application.Features.Auth.Dtos;

namespace QcmBackend.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<AuthResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
