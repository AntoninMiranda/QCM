using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QcmBackend.Application.Features.Auth.Dtos;

namespace QcmBackend.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        // DbSets will be added as features are implemented
        // Example: DbSet<Category> Categories { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public interface IPropertiesConfigurator
    {
        IEnumerable<string> GetDescriptiveProperties();
    }
    
}
