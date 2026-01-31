using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QcmBackend.Application.Features.Auth.Dtos;
using QcmBackend.Application.Common.Interfaces;

namespace QcmBackend.Application.Features.Auth.Commands
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName, request.Role);
            if (result != "User created successfully")
            {
                throw new Exception(result);
            }

            // After registration, login to get token
            return await _authService.LoginAsync(request.Email, request.Password);
        }
    }
}
