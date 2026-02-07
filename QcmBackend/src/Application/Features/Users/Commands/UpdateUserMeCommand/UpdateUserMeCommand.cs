using QcmBackend.Application.Common.Result;
using MediatR;

namespace QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand
{
    public class UpdateUserMeCommand : IRequest<Result>
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
    }
}