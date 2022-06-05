using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APIWithRabbitMQ.WebAPI.AppCode.Modules.NewsModule
{
    public class NewsGetAllActiveQuery : IRequest<IEnumerable<News>>
    {
        public class NewsGetAllActiveQueryHandler : IRequestHandler<NewsGetAllActiveQuery, IEnumerable<News>>
        {
            readonly RabbitDbContext db;

            public NewsGetAllActiveQueryHandler(RabbitDbContext db)
            {
                this.db = db;
            }

            async public Task<IEnumerable<News>> Handle(NewsGetAllActiveQuery request, CancellationToken cancellationToken)
            {
                return await db.News.Where(n => n.DeletedByUserId == null).ToListAsync(cancellationToken);
            }
        }
    }
}
