using MediatR;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Users.Dtos;

namespace QcmBackend.Application.Features.Users.Queries.GetUserMeQuery;

public class GetUserMeQueryHandler(IIdentityService identityService) : IRequestHandler<GetUserMeQuery, Result<ReadUserDto>>
{
    public async Task<Result<ReadUserDto>> Handle(GetUserMeQuery request, CancellationToken cancellationToken = default)
    {
        ReadUserDto? appUser = await identityService.GetUserMeAsync(cancellationToken);

        if (appUser == null)
        {
            return UsersErrors.NotFound(Guid.Empty);
        }

        return appUser;
    }
}
