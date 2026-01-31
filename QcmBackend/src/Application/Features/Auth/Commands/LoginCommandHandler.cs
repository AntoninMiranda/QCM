using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Features.Auth.Dtos;

namespace QcmBackend.Application.Features.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(request.Email, request.Password);
        }
    }
}
