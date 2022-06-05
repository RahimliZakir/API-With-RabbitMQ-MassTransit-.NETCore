using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APIWithRabbitMQ.WebAPI.AppCode.Modules.SubscriptionsModule
{
    public class SubscriptionSingleQuery : IRequest<SubscriptionViewModel>
    {
        public int? Id { get; set; }

        public class SubscriptionSingleQueryHandler : IRequestHandler<SubscriptionSingleQuery, SubscriptionViewModel>
        {
            readonly RabbitDbContext db;
            readonly IMapper mapper;

            public SubscriptionSingleQueryHandler(RabbitDbContext db, IMapper mapper)
            {
                this.db = db;
                this.mapper = mapper;
            }

            async public Task<SubscriptionViewModel> Handle(SubscriptionSingleQuery request, CancellationToken cancellationToken)
            {
                Subscription subscription = await db.Subscriptions.FirstOrDefaultAsync(s => s.Id == request.Id);

                SubscriptionViewModel viewModel = mapper.Map<SubscriptionViewModel>(subscription);

                return viewModel;
            }
        }
    }
}
