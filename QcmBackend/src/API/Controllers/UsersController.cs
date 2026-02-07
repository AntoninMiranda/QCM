using Asp.Versioning;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QcmBackend.API.Common.Extensions;
using QcmBackend.API.Requests.Users;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand;
using QcmBackend.Application.Features.Users.Dtos;
using QcmBackend.Application.Features.Users.Queries.GetUserMeQuery;
using QcmBackend.Application.Features.Users.Queries.GetUserQuery;

namespace QcmBackend.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class UsersController(IMediator mediator, IMapper mapper) : BaseController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        GetUserMeQuery query = new();

        Result<ReadUserDto> result = await mediator.Send(query);

        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }
    
    [HttpPut("me")]
    public async Task<ActionResult> UpdateUserMe([FromBody] UpdateUserMeRequest request)
    {
        UpdateUserMeCommand command = mapper.Map<UpdateUserMeCommand>(request);

        Result result = await mediator.Send(command);

        return result.Match(
            onSuccess: NoContent,
            onFailure: Problem
        );
    }
    
    [HttpGet("{userId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ReadUserDto>> GetUser([FromRoute] Guid userId)
    {
        GetUserQuery query = new()
        {
            UserId = userId
        };

        Result<ReadUserDto> result = await mediator.Send(query);

        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }
}
