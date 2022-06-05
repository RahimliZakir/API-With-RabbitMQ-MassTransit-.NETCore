using APIWithRabbitMQ.Domain.Models.DataContexts;
using APIWithRabbitMQ.Domain.Models.Entities.Membership;
using APIWithRabbitMQ.Domain.Models.Entities.Membership.Credentials;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace APIWithRabbitMQ.WebAPI.AppCode.Initializers
{
    public static class DatabaseInitializer
    {
        async public static Task<IApplicationBuilder> SeedData(this IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                RabbitDbContext db = scope.ServiceProvider.GetRequiredService<RabbitDbContext>();
                IActionContextAccessor ctx = scope.ServiceProvider.GetRequiredService<IActionContextAccessor>();
                RoleManager<AppRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
                UserManager<AppUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                db.Database.Migrate();

                AppRole roleResult = await roleManager.FindByNameAsync("Admin");

                //---Identity---
                if (roleResult == null)
                {
                    roleResult = new AppRole
                    {
                        Name = "Admin"
                    };

                    IdentityResult roleResponse = await roleManager.CreateAsync(roleResult);

                    if (roleResponse.Succeeded)
                    {
                        AppUser userResult = await userManager.FindByNameAsync("rahimlizakir");

                        if (userResult == null)
                        {
                            userResult = new AppUser
                            {
                                UserName = "rahimlizakir",
                                Email = "zakirer@code.edu.az"
                            };

                            IdentityResult userResponse = await userManager.CreateAsync(userResult, AdminCredential.Pick());

                            if (userResponse.Succeeded)
                            {
                                IdentityResult roleUserResult = await userManager.AddToRoleAsync(userResult, roleResult.Name);
                            }
                        }
                        else
                        {
                            IdentityResult roleUserResult = await userManager.AddToRoleAsync(userResult, roleResult.Name);
                        }
                    }
                }
                else
                {
                    AppUser userResult = await userManager.FindByNameAsync("rahimlizakir");

                    if (userResult == null)
                    {
                        userResult = new AppUser
                        {
                            UserName = "rahimlizakir",
                            Email = "zakirer@code.edu.az"
                        };

                        IdentityResult userResponse = await userManager.CreateAsync(userResult, AdminCredential.Pick());

                        if (userResponse.Succeeded)
                        {
                            IdentityResult roleUserResult = await userManager.AddToRoleAsync(userResult, roleResult.Name);
                        }
                    }
                    else
                    {
                        IdentityResult roleUserResult = await userManager.AddToRoleAsync(userResult, roleResult.Name);
                    }
                }
                //---Identity---
            }

            return app;
        }
    }
}
