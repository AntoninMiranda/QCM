using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using QcmBackend.Application.Common.Interfaces;

namespace QcmBackend.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest>(ILogger<TRequest> logger, IUser userContext) : IRequestPreProcessor<TRequest> where TRequest : notnull
    {
        public Task Process(TRequest request, CancellationToken cancellationToken = default)
        {
            string requestName = typeof(TRequest).Name;
            Guid userId = userContext.UserId;

            logger.LogInformation("CleanArchitecture Request: {Name} {@UserId} {@Request}",
                requestName, userId, request);

            return Task.CompletedTask;
        }
    }
}
