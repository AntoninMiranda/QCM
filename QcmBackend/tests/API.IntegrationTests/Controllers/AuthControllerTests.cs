using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using QcmBackend.API.Requests.Auth;
using QcmBackend.Application.Features.Auth.Dtos;
using QcmBackend.Tests.Common.Auth;

namespace QcmBackend.API.IntegrationTests.Controllers
{
    public class AuthControllerTests : ApiTestBase
    {
        [Fact]
        public async Task Register_Should_Return_Success()
        {
            ClearJwtToken();

            RegisterRequest request = AuthRequestsTestHelper.RegisterRequest();

            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Login_Should_Return_Token()
        {
            ClearJwtToken();

            LoginRequest loginRequest = AuthRequestsTestHelper.LoginRequest();
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

            _ = response.EnsureSuccessStatusCode();

            ReadTokenDto? tokenDto = await response.Content.ReadFromJsonAsync<ReadTokenDto>();

            _ = tokenDto.ShouldNotBeNull();
            tokenDto.AccessToken.ShouldNotBeNullOrEmpty();
            tokenDto.RefreshToken.ShouldNotBeNullOrEmpty();
        }
    }
}