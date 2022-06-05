using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APIWithRabbitMQ.WebAPI.AppCode.Modules.SubscriptionsModule
{
    public class SubscriptionGetByEmailQuery : IRequest<SubscriptionViewModel>
    {
        public string Email { get; set; }

        public class SubscriptionGetByEmailQueryHandler : IRequestHandler<SubscriptionGetByEmailQuery, SubscriptionViewModel>
        {
            readonly RabbitDbContext db;
            readonly IMapper mapper;

            public SubscriptionGetByEmailQueryHandler(RabbitDbContext db, IMapper mapper)
            {
                this.db = db;
                this.mapper = mapper;
            }

            async public Task<SubscriptionViewModel> Handle(SubscriptionGetByEmailQuery request, CancellationToken cancellationToken)
            {
                Subscription subscription = await db.Subscriptions.FirstOrDefaultAsync(s => s.Email == request.Email);

                SubscriptionViewModel viewModel = mapper.Map<SubscriptionViewModel>(subscription);

                return viewModel;
            }
        }
    }
}
