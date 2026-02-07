using MediatR;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Users.Dtos;

namespace QcmBackend.Application.Features.Users.Queries.GetUserQuery;

public class GetUserQueryHandler(IIdentityService identityService) : IRequestHandler<GetUserQuery, Result<ReadUserDto>>
{
    public async Task<Result<ReadUserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken = default)
    {
        ReadUserDto? appUser = await identityService.GetUserByIdAsync(request.UserId, cancellationToken);

        if (appUser == null)
        {
            return UsersErrors.NotFound(request.UserId);
        }

        return appUser;
    }
}