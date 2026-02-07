using Shouldly;
using Xunit;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Auth.Commands.LoginCommand;
using QcmBackend.Application.Features.Auth.Dtos;
using QcmBackend.Tests.Common.Auth;

namespace QcmBackend.Application.FunctionalTests.Auth.Commands;

public class LoginCommandFunctionalTests : ApplicationTestBase
{
    [Fact]
    public async Task LoginCommand_Should_Return_Token_Or_Failure()
    {
        LoginCommand command = AuthCommandsTestHelper.LoginCommand();

        Result<ReadTokenDto> result = await _mediator.Send(command);

        result.Succeeded.ShouldBeTrue();
        _ = result.Value.ShouldNotBeNull();
        result.Value.AccessToken.ShouldNotBeNullOrEmpty();
        result.Value.RefreshToken.ShouldNotBeNullOrEmpty();
    }
}
