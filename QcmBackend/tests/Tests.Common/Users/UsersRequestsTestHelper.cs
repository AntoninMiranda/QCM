using QcmBackend.API.Requests.Users;

namespace QcmBackend.Tests.Common.Users
{
    public class UsersRequestsTestHelper
    {
        public static UpdateUserMeRequest UpdateUserMeRequest(
            string firstName = "John",
            string lastName = "Doe"
        )
        {
            return new()
            {
                FirstName = firstName,
                LastName = lastName,
            };
        }
    }
}