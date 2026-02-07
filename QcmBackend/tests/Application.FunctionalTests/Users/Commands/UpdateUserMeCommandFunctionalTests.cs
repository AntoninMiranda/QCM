using Shouldly;
using Xunit;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand;
using QcmBackend.Tests.Common.Users;

namespace QcmBackend.Application.FunctionalTests.Users.Commands
{
    public class UpdateUserMeCommandFunctionalTests : ApplicationTestBase
    {
        [Fact]
        public async Task UpdateUserMeCommand_Should_Fail()
        {
            UpdateUserMeCommand command = UsersCommandsTestHelper.UpdateUserMeCommand();

            Result result = await _mediator.Send(command);

            result.Succeeded.ShouldBeFalse();
        }
    }
}