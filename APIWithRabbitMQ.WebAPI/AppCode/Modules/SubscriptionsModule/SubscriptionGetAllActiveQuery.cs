using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APIWithRabbitMQ.WebAPI.AppCode.Modules.SubscriptionsModule
{
    public class SubscriptionGetAllActiveQuery : IRequest<IEnumerable<Subscription>>
    {
        public class SubscriptionGetAllActiveQueryHandler : IRequestHandler<SubscriptionGetAllActiveQuery, IEnumerable<Subscription>>
        {
            readonly RabbitDbContext db;

            public SubscriptionGetAllActiveQueryHandler(RabbitDbContext db)
            {
                this.db = db;
            }

            public async Task<IEnumerable<Subscription>> Handle(SubscriptionGetAllActiveQuery request, CancellationToken cancellationToken)
            {
                IEnumerable<Subscription> subscriptions = await db.Subscriptions.Where(g => g.DeletedDate == null).ToListAsync(cancellationToken);

                return subscriptions;
            }
        }
    }
}
