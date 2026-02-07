using Shouldly;
using Xunit;
using QcmBackend.Application.Common.Result;
using QcmBackend.Application.Features.Users.Dtos;
using QcmBackend.Application.Features.Users.Queries.GetUserMeQuery;
using QcmBackend.Tests.Common.Users;

namespace QcmBackend.Application.FunctionalTests.Users.Queries
{
    public class GetUserMeQueryFunctionalTests : ApplicationTestBase
    {
        [Fact]
        public async Task GetMeQuery_Should_Fail()
        {
            GetUserMeQuery query = UsersQueriesTestHelper.GetUserMeQuery();

            Result<ReadUserDto> result = await _mediator.Send(query);

            result.Succeeded.ShouldBeFalse();
        }
    }
}