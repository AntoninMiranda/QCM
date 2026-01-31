using MediatR;
using QcmBackend.Application.Features.Auth.Dtos;


namespace QcmBackend.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<AuthResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
}
