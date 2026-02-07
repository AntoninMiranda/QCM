using AutoMapper;
using QcmBackend.Application.Features.Auth.Commands.RefreshTokenCommand;

namespace QcmBackend.API.Requests.Auth;

public class RefreshTokenRequest
{
    public string? RefreshToken { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            _ = CreateMap<RefreshTokenRequest, RefreshTokenCommand>();
        }
    }
}