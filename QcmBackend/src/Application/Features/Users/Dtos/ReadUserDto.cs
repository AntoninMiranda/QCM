using AutoMapper;
using QcmBackend.Domain.Interfaces;

namespace QcmBackend.Application.Features.Users.Dtos;

public class ReadUserDto
{
    public required Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            _ = CreateMap<IAppUser, ReadUserDto>();
        }
    }
}
