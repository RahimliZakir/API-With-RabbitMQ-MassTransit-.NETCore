using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities;
using APIWithRabbitMQ.WebAPI.AppCode.Extensions;
using APIWithRabbitMQ.WebAPI.AppCode.Infrastructure;
using APIWithRabbitMQ.WebAPI.AppCode.Mappers.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace APIWithRabbitMQ.WebAPI.AppCode.Modules.SubscriptionsModule
{
    public class SubscriptionCreateCommand : SubscriptionDto, IRequest<CommandJsonResponse>
    {
        public class SubscriptionCreateCommandHandler : IRequestHandler<SubscriptionCreateCommand, CommandJsonResponse>
        {
            readonly RabbitDbContext db;
            readonly IActionContextAccessor ctx;
            readonly IConfiguration config;
            readonly IMapper mapper;

            public SubscriptionCreateCommandHandler(RabbitDbContext db, IActionContextAccessor ctx, IConfiguration config, IMapper mapper)
            {
                this.db = db;
                this.ctx = ctx;
                this.config = config;
                this.mapper = mapper;
            }

            async public Task<CommandJsonResponse> Handle(SubscriptionCreateCommand request, CancellationToken cancellationToken)
            {
                CommandJsonResponse response = new();

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    response.Error = true;
                    response.Message = "Email boş göndərilə bilməz!";
                    goto end;
                }

                if (request.Email.IsEmail() == false)
                {
                    response.Error = true;
                    response.Message = "Xahiş olunur email formatında məlumat daxil edin!";
                    goto end;
                }

                Subscription isSubscribed = await db.Subscriptions.FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

                if (isSubscribed != null && isSubscribed.IsConfirmed)
                {
                    response.Error = true;
                    response.Message = "Siz artıq bizim abunəliyimizə qoşulmusunuz!";
                    goto end;
                }

                string token = $"{request.Email}-{DateTime.UtcNow.AddHours(4).AddMinutes(20):yyyyMMddHHmmss}";

                token = token.Encrypt();

                string url = $"{ctx.GetScheme()}://{ctx.GetHost()}/api/subscriptions/confirm-subscription/?token={token}";

                string fromMail = config.GetValue<string>("FactoryCredentials:Email");
                string pwd = config.GetValue<string>("FactoryCredentials:Password");
                string? cc = config.GetValue<string>("FactoryCredentials:CC");
                string subject = "Xahiş olunur abunəliyi təsdiq edin!";
                string body = $"Bu <a href='{url}'>linkə</a> klik edərək təsdiq pəncərəsinə yönələ bilərsiniz!";

                bool status = config.SendMail(fromMail, pwd, request.Email, subject, body, true, cc);

                if (!status)
                {
                    response.Error = true;
                    response.Message = "Təyin olunmayan xəta baş verdi, daha sonra yenidən yoxlayın!";
                    goto end;
                }


                Subscription data = mapper.Map<Subscription>(request);
                await db.Subscriptions.AddAsync(data, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                response.Error = false;
                response.Message = "Mail-inizə göndərilmiş linkə keçid edərək, mail-inizi təsdiq edin!";

            end:
                return response;
            }
        }
    }
}
