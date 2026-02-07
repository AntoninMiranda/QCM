using MediatR;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Auth.Dtos;

namespace QcmBackend.Application.Features.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommandHandler(IIdentityService identityService) : IRequestHandler<RefreshTokenCommand, Result<ReadTokenDto>>
{
    public async Task<Result<ReadTokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        return await identityService.RefreshTokenAsync(request, cancellationToken);
    }
}