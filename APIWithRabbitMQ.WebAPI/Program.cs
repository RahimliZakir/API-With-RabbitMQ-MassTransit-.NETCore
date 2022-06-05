using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities.Membership;
using APIWithRabbitMQ.Domain.Models.FormModels;
using APIWithRabbitMQ.WebAPI.AppCode.Consumers;
using APIWithRabbitMQ.WebAPI.AppCode.Extensions;
using APIWithRabbitMQ.WebAPI.AppCode.Initializers;
using APIWithRabbitMQ.WebAPI.AppCode.Providers;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration conf = builder.Configuration;

IServiceCollection services = builder.Services;
services.AddRouting(cfg =>
{
    cfg.LowercaseUrls = true;
});

services.AddControllers(cfg =>
{
    AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                                     .RequireAuthenticatedUser()
                                     .Build();

    cfg.Filters.Add(new AuthorizeFilter(policy));
}).AddNewtonsoftJson(cfg => cfg.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

// RabbitMQ Config
services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<NewsConsumer>();

    cfg.AddBus(provider =>
    {
        IBusControl bus = Bus.Factory.CreateUsingRabbitMq(b =>
        {
            b.Host(new Uri("rabbitmq://localhost"), h =>
            {
                h.Username("guest");
                h.Password("12345");
            });

            // Configure Routing Key, Exchange Name & Exchange Type.
            b.Send<SubscribeFormModel>(x => { x.UseRoutingKeyFormatter(context => "account.init"); });
            b.Message<SubscribeFormModel>(x => x.SetEntityName("subscribe-message-send"));
            b.Publish<SubscribeFormModel>(x => { x.ExchangeType = ExchangeType.Direct; });
            // Configure Routing Key, Exchange Name & Exchange Type.

            b.ReceiveEndpoint("newsQueue", r =>
            {
                r.PrefetchCount = 16;
                r.UseMessageRetry(i => i.Interval(3, 2000));
                r.ConfigureConsumer<NewsConsumer>(provider);

                // Bind Routing Key, Exchange Name & Exchange Type.
                r.Bind("subscribe-message-send", x =>
                {
                    x.ExchangeType = ExchangeType.Direct;
                    x.RoutingKey = "account.init";
                });
                // Bind Routing Key, Exchange Name & Exchange Type.
            });
        });

        return bus;
    });
});
// RabbitMQ Config

services.AddDbContext<RabbitDbContext>();

services.AddIdentity<AppUser, AppRole>()
        .AddEntityFrameworkStores<RabbitDbContext>()
        .AddDefaultTokenProviders();

services.AddScoped<RoleManager<AppRole>>()
        .AddScoped<UserManager<AppUser>>()
        .AddScoped<SignInManager<AppUser>>();

services.AddScoped<IClaimsTransformation, AppClaimProvider>();

services.AddMediatR(typeof(Program));
services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

services.AddAutoMapper(typeof(Program));

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API With RabbitMQ",
        Description = "ASP.Net Core API with RabbitMQ",
        //TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Zakir Rahimli",
            Email = "zakirer@code.edu.az",
            Url = new Uri("https://api.p313.az"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://example.com/license"),
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
              new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
        }
    });


    c.EnableAnnotations();
});

services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";
    options.User.RequireUniqueEmail = true;
});

byte[] buffer = Encoding.UTF8.GetBytes(conf.GetValue<string>("Jwt:Secret"));

services.AddAuthentication(cfg =>
{
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = conf.GetValue<string>("Jwt:Issuer"),
        ValidAudience = conf.GetValue<string>("Jwt:Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(buffer)
    };
});

services.AddAuthorization(cfg =>
{
    string[] principals = services.GetPrincipals(typeof(Program));

    foreach (string principal in principals)
    {
        cfg.AddPolicy(principal, options =>
        {
            options.RequireAssertion(assertion =>
            {
                return assertion.User.IsInRole("Admin") && assertion.User.HasClaim(principal, "1");
            });
        });
    }
});

WebApplication app = builder.Build();
IWebHostEnvironment env = builder.Environment;
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// RabbitMQ Config
IServiceProvider Configure(IApplicationBuilder app)
{
    return app.ApplicationServices;
}

IServiceProvider serviceProvider = Configure(app);
IBusControl bus = serviceProvider.GetRequiredService<IBusControl>();
// RabbitMQ Config

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));

app.UseStaticFiles();

app.SeedData().Wait();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
