using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using APIWithRabbitMQ.Domain.Models.FormModels;
using APIWithRabbitMQ.WebAPI.AppCode.Extensions;
using APIWithRabbitMQ.WebAPI.AppCode.Infrastructure;
using APIWithRabbitMQ.WebAPI.AppCode.Mappers.Dtos;
using AutoMapper;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace APIWithRabbitMQ.WebAPI.AppCode.Modules.NewsModule
{
    public class NewsCreateCommand : NewsDto, IRequest<CommandJsonResponse>
    {
        public class NewsCreateCommandHandler : IRequestHandler<NewsCreateCommand, CommandJsonResponse>
        {
            readonly RabbitDbContext db;
            readonly IActionContextAccessor ctx;
            // RabbitMQ
            readonly IBus bus;
            // RabbitMQ
            readonly IMapper mapper;
            readonly IConfiguration conf;

            public NewsCreateCommandHandler(RabbitDbContext db, IActionContextAccessor ctx, IBus bus, IMapper mapper, IConfiguration conf)
            {
                this.db = db;
                this.ctx = ctx;
                // RabbitMQ
                this.bus = bus;
                // RabbitMQ
                this.mapper = mapper;
                this.conf = conf;
            }

            async public Task<CommandJsonResponse> Handle(NewsCreateCommand request, CancellationToken cancellationToken)
            {
                CommandJsonResponse response = new();

                if (ctx.IsValid())
                {
                    // RabbitMQ
                    Uri uri = new("rabbitmq://localhost/newsQueue");
                    ISendEndpoint endpoint = await bus.GetSendEndpoint(uri);

                    Subscription[] subscriptions = await db.Subscriptions.Where(s => s.DeletedByUserId == null).ToArrayAsync(cancellationToken);
                    SubscribeFormModel formModel = new();

                    News news = mapper.Map<News>(request);
                    news.CreatedByUserId = ctx.GetUserId();

                    for (int i = 0; i < subscriptions.Length; i++)
                    {
                        formModel.Subscription = subscriptions[i];
                        formModel.News = news;

                        //await endpoint.Send(formModel, cancellationToken);

                        await bus.Publish(formModel, cancellationToken);
                    }
                    // RabbitMQ

                    // -- We'll set datas from Producer to Consumer with the help of RabbitMQ.
                    //await db.News.AddAsync(news, cancellationToken);
                    //await db.SaveChangesAsync(cancellationToken);

                    response.Error = false;
                    response.Message = "Yeni xəbərlər abunəçilərə uğurla göndərildi!";
                    goto end;
                }

                response.Error = true;
                response.Message = "Məlumatlar əlavə olunan zaman xəta baş verdi!";

            end:
                return response;
            }
        }
    }
}
