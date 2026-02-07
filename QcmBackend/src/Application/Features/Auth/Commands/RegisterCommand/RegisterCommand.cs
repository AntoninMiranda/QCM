using QcmBackend.Application.Common.Result;
using MediatR;

namespace QcmBackend.Application.Features.Auth.Commands.RegisterCommand;

public class RegisterCommand : IRequest<Result>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
