using AutoMapper;
using QcmBackend.Application.Features.Auth.Commands.RegisterCommand;

namespace QcmBackend.API.Requests.Auth;

public class RegisterRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            _ = CreateMap<RegisterRequest, RegisterCommand>();
        }
    }
}