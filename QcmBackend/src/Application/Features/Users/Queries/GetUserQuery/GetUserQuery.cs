using MediatR;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Users.Dtos;

namespace QcmBackend.Application.Features.Users.Queries.GetUserQuery;

public class GetUserQuery : IRequest<Result<ReadUserDto>>
{
    public required Guid UserId { get; init; }
}