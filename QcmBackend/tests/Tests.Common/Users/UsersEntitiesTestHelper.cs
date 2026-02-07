using QcmBackend.Tests.Common.Moqs;

namespace QcmBackend.Tests.Common.Users
{
    public static class UsersEntitiesTestHelper
    {
        public static FakeAppUser CreateValidUser(
            Guid? id = null,
            string email = "Email",
            string firstName = "Tom",
            string lastName = "Etnana",
            DateTimeOffset? createdAt = null
        )
        {
            return new()
            {
                Id = id ?? Guid.NewGuid(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = createdAt ?? DateTimeOffset.UtcNow
            };
        }
    }
}