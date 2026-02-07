using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Auth.Commands.LoginCommand;
using QcmBackend.Application.Features.Auth.Commands.RefreshTokenCommand;
using QcmBackend.Application.Features.Auth.Commands.RegisterCommand;
using QcmBackend.Application.Features.Auth.Dtos;
using QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand;
using QcmBackend.Application.Features.Users.Dtos;

namespace QcmBackend.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result.Result> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default);
    Task<Result<ReadTokenDto>> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default);
    // Task<Result.Result> LogoutAsync(LogoutCommand command, CancellationToken cancellationToken = default);
    Task<Result<ReadTokenDto>> RefreshTokenAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default);
    Task<Result.Result> UpdateUserMeAsync(UpdateUserMeCommand command, CancellationToken cancellationToken = default);
    Task<ReadUserDto?> GetUserMeAsync(CancellationToken cancellationToken = default);
    Task<ReadUserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<ReadUserDto>> GetUsersByIdsAsync(List<Guid> userIds, CancellationToken cancellationToken = default);
}