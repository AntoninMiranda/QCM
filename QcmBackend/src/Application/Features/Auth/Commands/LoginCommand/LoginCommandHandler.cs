using MediatR;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Auth.Dtos;

namespace QcmBackend.Application.Features.Auth.Commands.LoginCommand
{
    public class LoginCommandHandler(IIdentityService identityService) : IRequestHandler<LoginCommand, Result<ReadTokenDto>>
    {
        public async Task<Result<ReadTokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken = default)
        {
            return await identityService.LoginAsync(request, cancellationToken);
        }
    }
}
