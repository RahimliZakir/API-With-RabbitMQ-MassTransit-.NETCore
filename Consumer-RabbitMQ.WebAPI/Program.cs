using APIWithRabbitMQ.Domain.Models.DataContexts;
using Autofac;
using Consumer_RabbitMQ.WebAPI.AppCode.Consumers;
using MassTransit;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

services.AddControllers();

services.AddEndpointsApiExplorer();

services.AddSwaggerGen();

services.AddSingleton<IConsumer, NewsConsumer>();
services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<NewsConsumer>();

    cfg.AddBus(provider =>
    {
        IBusControl bus = Bus.Factory.CreateUsingRabbitMq(config =>
         {
             config.Host(new Uri("rabbitmq://localhost"), h =>
             {
                 h.Username("guest");
                 h.Password("12345");
             });

             config.ReceiveEndpoint("newsQueue", ep =>
             {
                 ep.PrefetchCount = 16;
                 ep.UseMessageRetry(r => r.Interval(2, 100));
                 ep.ConfigureConsumer<NewsConsumer>(provider);
             });
         });

        return bus;
    });
});

IServiceProvider serviceProvider = services.BuildServiceProvider();
IBusControl? bus = serviceProvider.GetService<IBusControl>();

WebApplication app = builder.Build();
IWebHostEnvironment env = builder.Environment;

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
