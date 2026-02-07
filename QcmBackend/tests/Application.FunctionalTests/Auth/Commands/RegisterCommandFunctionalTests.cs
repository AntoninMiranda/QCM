using Xunit;
using Shouldly;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Auth.Commands.RegisterCommand;
using QcmBackend.Tests.Common.Auth;

namespace QcmBackend.Application.FunctionalTests.Auth.Commands
{
    public class RegisterCommandFunctionalTests : ApplicationTestBase
    {
        [Fact]
        public async Task RegisterCommand_Should_Return_Success()
        {
            RegisterCommand command = AuthCommandsTestHelper.RegisterCommand();

            Result result = await _mediator.Send(command);

            result.Succeeded.ShouldBeTrue();
        }
    }
}