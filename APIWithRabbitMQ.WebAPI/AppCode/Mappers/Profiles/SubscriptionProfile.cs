using APIWithRabbitMQ.Domain.Models.Entities;
using APIWithRabbitMQ.WebAPI.AppCode.Modules.SubscriptionsModule;
using AutoMapper;

namespace APIWithRabbitMQ.WebAPI.AppCode.Mappers.Profiles
{
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            CreateMap<SubscriptionCreateCommand, Subscription>();
            CreateMap<Subscription, SubscriptionViewModel>();
        }
    }
}
