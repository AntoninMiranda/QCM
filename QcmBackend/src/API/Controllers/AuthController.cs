using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QcmBackend.API.Common.Attributes;
using QcmBackend.API.Requests.Auth;
using QcmBackend.API.Common.Extensions;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Auth.Commands.LoginCommand;
using QcmBackend.Application.Features.Auth.Commands.RegisterCommand;
using QcmBackend.Application.Features.Auth.Dtos;

using LoginRequest = QcmBackend.API.Requests.Auth.LoginRequest;
using RegisterRequest = QcmBackend.API.Requests.Auth.RegisterRequest;

namespace QcmBackend.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[AllowAnonymous]
[EnableRateLimiting("auth")]
public class AuthController(IMediator mediator, IMapper mapper) : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        RegisterCommand command = mapper.Map<RegisterCommand>(request);

        Result result = await mediator.Send(command);

        return result.Match(
            onSuccess: NoContent,
            onFailure: Problem
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        LoginCommand command = mapper.Map<LoginCommand>(request);

        Result<ReadTokenDto> result = await mediator.Send(command);

        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }
}
