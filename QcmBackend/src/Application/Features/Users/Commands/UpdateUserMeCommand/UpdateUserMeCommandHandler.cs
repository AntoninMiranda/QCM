using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Common.Result;
using MediatR;

namespace QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand;

public class UpdateUserMeCommandCommandHandler(IIdentityService identityService) : IRequestHandler<UpdateUserMeCommand, Result>
{
    public async Task<Result> Handle(UpdateUserMeCommand request, CancellationToken cancellationToken = default)
    {
        return await identityService.UpdateUserMeAsync(request, cancellationToken);
    }
}
