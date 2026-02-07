using FluentValidation.TestHelper;
using Xunit;
using QcmBackend.Application.Common.Validation;
using QcmBackend.Application.Features.Users.Commands.UpdateUserMeCommand;
using QcmBackend.Tests.Common.Users;

namespace QcmBackend.Application.FunctionalTests.Users.Validators
{
    public class UpdateUserMeCommandValidatorFunctionalTests
    {
        private readonly UpdateUserMeCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_FirstName_IsEmpty()
        {
            UpdateUserMeCommand command = UsersCommandsTestHelper.UpdateUserMeCommand(firstName: "");

            TestValidationResult<UpdateUserMeCommand> result = _validator.TestValidate(command);

            _ = result.ShouldHaveValidationErrorFor(x => x.FirstName)
                  .WithErrorCode(ValidationCodes.Required.ToString());
        }

        [Fact]
        public void Should_Not_Have_Error_When_FirstName_ContainsAccents()
        {
            UpdateUserMeCommand command = UsersCommandsTestHelper.UpdateUserMeCommand(firstName: "NömÄâccëntéàè");

            TestValidationResult<UpdateUserMeCommand> result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_LastName_IsEmpty()
        {
            UpdateUserMeCommand command = UsersCommandsTestHelper.UpdateUserMeCommand(lastName: "");

            TestValidationResult<UpdateUserMeCommand> result = _validator.TestValidate(command);

            _ = result.ShouldHaveValidationErrorFor(x => x.LastName)
                  .WithErrorCode(ValidationCodes.Required.ToString());
        }

        [Fact]
        public void Should_Not_Have_Error_When_LastName_ContainsAccents()
        {
            UpdateUserMeCommand command = UsersCommandsTestHelper.UpdateUserMeCommand(lastName: "NömÄâccëntéàè");

            TestValidationResult<UpdateUserMeCommand> result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_For_Valid_Command()
        {
            UpdateUserMeCommand command = UsersCommandsTestHelper.UpdateUserMeCommand();

            TestValidationResult<UpdateUserMeCommand> result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}