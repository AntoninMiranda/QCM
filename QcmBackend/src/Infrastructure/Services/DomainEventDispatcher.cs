using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using QcmBackend.Infrastructure.Data.Interceptors;

namespace QcmBackend.Infrastructure.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchAsync(IEnumerable<object> events)
        {
            foreach (var @event in events)
            {
                await _mediator.Publish(@event);
            }
        }
    }
}
