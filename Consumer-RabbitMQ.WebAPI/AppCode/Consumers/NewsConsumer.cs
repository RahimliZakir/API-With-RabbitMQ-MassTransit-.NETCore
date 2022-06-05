using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using MassTransit;

namespace Consumer_RabbitMQ.WebAPI.AppCode.Consumers
{
    public class NewsConsumer : IConsumer<News>
    {
        readonly RabbitDbContext db;

        public NewsConsumer(RabbitDbContext db)
        {
            this.db = db;
        }

        public async Task Consume(ConsumeContext<News> context)
        {
            CancellationToken cancellationToken = context.CancellationToken;
            //CancellationTokenSource source = new();
            //CancellationToken token = source.Token;

            News data = context.Message;

            await db.News.AddAsync(data, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            await Task.FromResult(true);
        }
    }
}
