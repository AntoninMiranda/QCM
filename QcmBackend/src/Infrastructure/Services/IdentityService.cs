using AutoMapper;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Common.Settings;
using QcmBackend.Application.Common.Utils;
using QcmBackend.Application.Features.Auth.Commands.LoginCommand;
// using QcmBackend.Application.Features.Auth.Commands.LogoutCommand;
// using QcmBackend.Application.Features.Auth.Commands.RefreshTokenCommand;
using QcmBackend.Application.Features.Auth.Commands.RegisterCommand;
using QcmBackend.Application.Features.Auth.Dtos;
// using QcmBackend.Application.Features.Users;
// using QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand;
// using QcmBackend.Application.Features.Users.Dtos;
using QcmBackend.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QcmBackend.Application.Features.Auth.Commands;
using QcmBackend.Application.Features.Auth.Commands.RefreshTokenCommand;
using QcmBackend.Application.Features.Users;
using QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand;
using QcmBackend.Application.Features.Users.Dtos;

namespace QcmBackend.Infrastructure.Services;

public class IdentityService(
    UserManager<AppUser> userManager,
    ICookieService cookieService,
    IUser userContext,
    ITokenService tokenService,
    IOptions<SecuritySettings> securityOptions,
    IOptions<GeneralSettings> generalOptions,
    IOptions<AuthSettings> jwtOptions,
    TimeProvider timeProvider,
    IMapper mapper
) : IIdentityService
{
    private readonly SecuritySettings _securitySettings = securityOptions.Value;
    private readonly GeneralSettings _generalSettings = generalOptions.Value;
    private readonly AuthSettings _jwtSettings = jwtOptions.Value;

    public async Task<Result> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        AppUser? existingUser = await userManager.FindByEmailAsync(command.Email);
        if (existingUser != null)
        {
            return UsersErrors.Conflict("email");
        }

        AppUser user = new()
        {
            UserName = command.Email,
            Email = command.Email,
            FirstName = command.FirstName.CapitalizeProperName(),
            LastName = command.LastName.CapitalizeProperName(),
        };

        IdentityResult createResult = await userManager.CreateAsync(user, command.Password);

        if (!createResult.Succeeded)
        {
            return UsersErrors.Failure();
        }

        IdentityResult addToRoleResult = await userManager.AddToRoleAsync(user, "User");

        if (!addToRoleResult.Succeeded)
        {
            return UsersErrors.Failure();
        }

        return Result.Success();
    }

    public async Task<Result<ReadTokenDto>> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        AppUser? user = await userManager.FindByEmailAsync(command.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, command.Password))
        {
            return UsersErrors.InvalidCredentials();
        }

        IList<string> roles = await userManager.GetRolesAsync(user);
        string token = tokenService.GenerateAccessToken(user.Id, user.Email!, user.FirstName, user.LastName, roles);
        string refreshToken = tokenService.GenerateRefreshToken();
        string refreshTokenComposite = tokenService.GenerateRefreshTokenComposite(user.Id, refreshToken);

        int refreshExpiresInDays = command.RememberMe
           ? _jwtSettings.RememberMeRefreshExpiresInDays
           : _jwtSettings.DefaultRefreshExpiresInDays;

        user.RefreshTokenHash = tokenService.Hash(refreshToken);
        user.RefreshTokenExpiryTime = timeProvider.GetUtcNow().AddDays(refreshExpiresInDays);
        user.PreviousRefreshTokenHash = null;
        user.PreviousRefreshTokenValidUntil = null;

        IdentityResult result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return UsersErrors.Failure();
        }

        cookieService.SetRefreshToken(refreshTokenComposite, user.RefreshTokenExpiryTime!.Value);

        return new ReadTokenDto()
        {
            AccessToken = token,
            RefreshToken = refreshTokenComposite
        };
    }
    
    public async Task<Result<ReadTokenDto>> RefreshTokenAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        string? refreshTokenComposite = command?.RefreshToken ?? cookieService.GetRefreshToken();

        if (refreshTokenComposite == null)
        {
            return UsersErrors.InvalidToken();
        }

        string[] split = refreshTokenComposite.Split('.', 2);

        if (!Guid.TryParse(split[0], out Guid userId))
        {
            return UsersErrors.InvalidToken();
        }

        AppUser? user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return UsersErrors.InvalidToken();
        }

        DateTimeOffset now = timeProvider.GetUtcNow();

        bool matchesCurrent =
            user.RefreshTokenHash != null
            && user.RefreshTokenExpiryTime != null
            && user.RefreshTokenExpiryTime >= now
            && tokenService.Verify(split[1], user.RefreshTokenHash);

        bool matchesPrevious =
            user.PreviousRefreshTokenHash != null
            && user.PreviousRefreshTokenValidUntil != null
            && now <= user.PreviousRefreshTokenValidUntil
            && tokenService.Verify(split[1], user.PreviousRefreshTokenHash);

        if (!matchesCurrent && !matchesPrevious)
        {
            return UsersErrors.InvalidToken();
        }

        IList<string> roles = await userManager.GetRolesAsync(user);
        string newToken = tokenService.GenerateAccessToken(user.Id, user.Email!, user.FirstName, user.LastName, roles);

        if (matchesCurrent)
        {
            string newRefreshToken = tokenService.GenerateRefreshToken();
            string newRefreshTokenComposite = tokenService.GenerateRefreshTokenComposite(user.Id, newRefreshToken);

            user.PreviousRefreshTokenHash = user.RefreshTokenHash;
            user.PreviousRefreshTokenValidUntil = now.AddSeconds(_securitySettings.RefreshReuseLeewaySeconds);

            user.RefreshTokenHash = tokenService.Hash(newRefreshToken);

            IdentityResult result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return UsersErrors.Failure();
            }

            cookieService.SetRefreshToken(newRefreshTokenComposite, user.RefreshTokenExpiryTime!.Value);

            return new ReadTokenDto()
            {
                AccessToken = newToken,
                RefreshToken = newRefreshTokenComposite
            };
        }

        return new ReadTokenDto()
        {
            AccessToken = newToken
        };
    }
    
    public async Task<ReadUserDto?> GetUserMeAsync(CancellationToken cancellationToken = default)
    {
        AppUser? user = await userManager.FindByIdAsync(userContext.UserId.ToString());

        if (user == null)
        {
            return null;
        }

        ReadUserDto userDto = mapper.Map<ReadUserDto>(user);

        return userDto;
    }

    public async Task<ReadUserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        AppUser? user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return null;
        }

        ReadUserDto userDto = mapper.Map<ReadUserDto>(user);

        return userDto;
    }
    
    public async Task<List<ReadUserDto>> GetUsersByIdsAsync(List<Guid> userIds, CancellationToken cancellationToken = default)
    {
        List<AppUser> users = await userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        List<ReadUserDto> userDtos = mapper.Map<List<ReadUserDto>>(users);

        return userDtos;
    }
    
    public async Task<Result> UpdateUserMeAsync(UpdateUserMeCommand command, CancellationToken cancellationToken = default)
    {
        AppUser? user = await userManager.FindByIdAsync(userContext.UserId.ToString());

        if (user == null)
        {
            return UsersErrors.NotFound(userContext.UserId);
        }

        user.FirstName = command.FirstName.CapitalizeProperName();
        user.LastName = command.LastName.CapitalizeProperName();

        IdentityResult updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return UsersErrors.Failure();
        }

        return Result.Success();
    }

}
