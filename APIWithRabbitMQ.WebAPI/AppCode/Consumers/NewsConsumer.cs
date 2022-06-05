using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using APIWithRabbitMQ.Domain.Models.FormModels;
using APIWithRabbitMQ.WebAPI.AppCode.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace APIWithRabbitMQ.WebAPI.AppCode.Consumers
{
    public class NewsConsumer : IConsumer<SubscribeFormModel>
    {
        readonly RabbitDbContext db;
        readonly IConfiguration conf;

        public NewsConsumer(RabbitDbContext db, IConfiguration conf)
        {
            this.db = db;
            this.conf = conf;
        }

        async public Task Consume(ConsumeContext<SubscribeFormModel> context)
        {
            CancellationToken cancellationToken = context.CancellationToken;

            News news = context.Message.News;
            Subscription subscription = context.Message.Subscription;

            string fromMail = conf.GetValue<string>("FactoryCredentials:Email");
            string pwd = conf.GetValue<string>("FactoryCredentials:Password");
            string? cc = conf.GetValue<string>("FactoryCredentials:CC");
            string subject = $"{news.Title}";
            string body = $@"<div style='text-align: center;'>
                               <h1>{news.Content}</h1>
                               <p>{DateTime.UtcNow.AddHours(4):dd.MM.yyyy HH.mm.ss}</p>
                             </div>";

            bool status = conf.SendMail(fromMail, pwd, subscription.Email, subject, body, true, cc);

            if (!await db.News.AnyAsync(n => n.Title.Equals(news.Title) && n.Content.Equals(news.Content), cancellationToken))
            {
                await db.News.AddAsync(news, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
