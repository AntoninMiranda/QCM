using QcmBackend.API.Requests.Auth;

namespace QcmBackend.Tests.Common.Auth
{
    public class AuthRequestsTestHelper
    {
        public static RegisterRequest RegisterRequest(
            string email = "fake.email@example.com",
            string password = "Password123!",
            string firstName = "John",
            string lastName = "Doe"
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

        public static LoginRequest LoginRequest(
            string email = "admin@example.com",
            string password = "Admin123!",
            bool rememberMe = false
        )
        {
            return new()
            {
                Email = email,
                Password = password,
                RememberMe = rememberMe
            };
        }
    }
}