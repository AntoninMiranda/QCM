using QcmBackend.Application.Features.Auth.Commands.LoginCommand;
using QcmBackend.Application.Features.Auth.Commands.RefreshTokenCommand;
using QcmBackend.Application.Features.Auth.Commands.RegisterCommand;

namespace QcmBackend.Tests.Common.Auth
{
    public static class AuthCommandsTestHelper
    {
        public static RegisterCommand RegisterCommand(
            string email = "validemail@example.com",
            string password = "Password1234",
            string firstName = "Tom",
            string lastName = "Etnana"
        )
        {
            return new()
            {
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName
            };
        }
        
        public static LoginCommand LoginCommand(
            string email = "admin@example.com",
            string password = "Admin123!",
            bool remember = false
        )
        {
            return new()
            {
                Email = email,
                Password = password,
                RememberMe = remember
            };
        }

        public static RefreshTokenCommand RefreshTokenCommand(string? refreshToken = null)
        {
            return new()
            {
                RefreshToken = refreshToken
            };
        }
    }
}