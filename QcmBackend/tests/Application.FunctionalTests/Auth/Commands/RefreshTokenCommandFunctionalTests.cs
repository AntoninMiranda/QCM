using Xunit;
using Shouldly;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Auth.Commands.RefreshTokenCommand;
using QcmBackend.Application.Features.Auth.Dtos;
using QcmBackend.Tests.Common.Auth;

namespace QcmBackend.Application.FunctionalTests.Auth.Commands
{
    public class RefreshTokenCommandFunctionalTests : ApplicationTestBase
    {
        [Fact]
        public async Task RefreshTokenCommand_Should_Return_Failure_When_No_RefreshToken()
        {
            RefreshTokenCommand command = AuthCommandsTestHelper.RefreshTokenCommand();

            Result<ReadTokenDto> result = await _mediator.Send(command);

            result.Succeeded.ShouldBeFalse();
        }
    }
}