using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Common.Result;
using MediatR;

namespace QcmBackend.Application.Features.Auth.Commands.RegisterCommand;

public class RegisterCommandHandler(IIdentityService identityService) : IRequestHandler<RegisterCommand, Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken = default)
    {
        return await identityService.RegisterAsync(request, cancellationToken);
    }
}