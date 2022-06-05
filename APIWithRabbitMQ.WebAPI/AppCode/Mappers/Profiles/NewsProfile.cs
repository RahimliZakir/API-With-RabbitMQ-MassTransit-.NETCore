using APIWithRabbitMQ.Domain.Models.Entities;
using APIWithRabbitMQ.WebAPI.AppCode.Modules.NewsModule;
using AutoMapper;

namespace APIWithRabbitMQ.WebAPI.AppCode.Mappers.Profiles
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<NewsCreateCommand, News>();
        }
    }
}
