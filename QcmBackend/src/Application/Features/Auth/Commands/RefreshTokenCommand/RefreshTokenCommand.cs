using MediatR;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Auth.Dtos;

namespace QcmBackend.Application.Features.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommand : IRequest<Result<ReadTokenDto>>
{
    public string? RefreshToken { get; set; }
}