using QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand;

namespace QcmBackend.Tests.Common.Users
{
    public static class UsersCommandsTestHelper
    {
        public static UpdateUserMeCommand UpdateUserMeCommand(
            string firstName = "Tom",
            string lastName = "Etnana"
        )
        {
            return new()
            {
                FirstName = firstName,
                LastName = lastName,
            };
        }
    }
}