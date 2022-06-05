using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APIWithRabbitMQ.WebAPI.AppCode.Modules.NewsModule
{
    public class NewsSingleQuery : IRequest<News>
    {
        public int? Id { get; set; }

        public class NewsSingleQueryHandler : IRequestHandler<NewsSingleQuery, News>
        {
            readonly RabbitDbContext db;

            public NewsSingleQueryHandler(RabbitDbContext db)
            {
                this.db = db;
            }

            public async Task<News> Handle(NewsSingleQuery request, CancellationToken cancellationToken)
            {
                News news = await db.News.FirstOrDefaultAsync(n => n.Id == request.Id && n.DeletedByUserId == null, cancellationToken);

                return news;
            }
        }
    }
}
