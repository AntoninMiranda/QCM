using AutoMapper;
using QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand;

namespace QcmBackend.API.Requests.Users;

public class UpdateUserMeRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            _ = CreateMap<UpdateUserMeRequest, UpdateUserMeCommand>();
        }
    }
}
