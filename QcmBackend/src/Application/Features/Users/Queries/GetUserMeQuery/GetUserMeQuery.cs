using MediatR;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Users.Dtos;

namespace QcmBackend.Application.Features.Users.Queries.GetUserMeQuery;

public class GetUserMeQuery : IRequest<Result<ReadUserDto>>
{
}