using FluentValidation.TestHelper;
using Xunit;
using QcmBackend.Application.Features.Users.Queries.GetUserMeQuery;
using QcmBackend.Tests.Common.Users;

namespace QcmBackend.Application.FunctionalTests.Users.Validators
{
    public class GetUserMeQueryValidatorFunctionalTests
    {
        private readonly GetUserMeQueryValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_For_Valid_Query()
        {
            GetUserMeQuery query = UsersQueriesTestHelper.GetUserMeQuery();

            TestValidationResult<GetUserMeQuery> result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}