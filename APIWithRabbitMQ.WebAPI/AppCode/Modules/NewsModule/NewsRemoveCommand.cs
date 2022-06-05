using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using APIWithRabbitMQ.WebAPI.AppCode.Extensions;
using APIWithRabbitMQ.WebAPI.AppCode.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace APIWithRabbitMQ.WebAPI.AppCode.Modules.NewsModule
{
    public class NewsRemoveCommand : IRequest<CommandJsonResponse>
    {
        public int? Id { get; set; }

        public class NewsDeleteCommandHandler : IRequestHandler<NewsRemoveCommand, CommandJsonResponse>
        {
            readonly RabbitDbContext db;
            readonly IActionContextAccessor ctx;

            public NewsDeleteCommandHandler(RabbitDbContext db, IActionContextAccessor ctx)
            {
                this.db = db;
                this.ctx = ctx;
            }

            async public Task<CommandJsonResponse> Handle(NewsRemoveCommand request, CancellationToken cancellationToken)
            {
                CommandJsonResponse response = new();

                if (request.Id != null || request.Id <= 0)
                {
                    response.Error = true;
                    response.Message = "Məlumtın tamlığı qorunmayıb!";
                    goto stop;
                }

                News entity = await db.News.FirstOrDefaultAsync(n => n.Id.Equals(request.Id) && n.DeletedByUserId == null, cancellationToken);

                if (entity == null)
                {
                    response.Error = true;
                    response.Message = "Belə bir məlumat yoxdur!";
                    goto stop;
                }

                entity.DeletedDate = DateTime.UtcNow.AddHours(4);
                entity.DeletedByUserId = ctx.GetUserId();

                response.Error = false;
                response.Message = "Məlumat uğurla silindi!";

            stop:
                return response;
            }
        }
    }
}
