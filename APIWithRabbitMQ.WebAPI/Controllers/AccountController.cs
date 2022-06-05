using APIWithRabbitMQ.Domain.Models.Entities.Membership;
using APIWithRabbitMQ.Domain.Models.FormModels;
using APIWithRabbitMQ.WebAPI.AppCode.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace APIWithRabbitMQ.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly UserManager<AppUser> userManager;
        readonly SignInManager<AppUser> signInManager;
        readonly IConfiguration conf;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration conf)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.conf = conf;
        }

        [AllowAnonymous]
        [HttpPost]
        async public Task<IActionResult> Login(LoginFormModel formModel)
        {
            if (string.IsNullOrWhiteSpace(formModel.Username))
            {
                return BadRequest(new
                {
                    error = true,
                    message = "İstifadəçi adı göndərilməyib!"
                });
            }

            if (string.IsNullOrWhiteSpace(formModel.Password))
            {
                return BadRequest(new
                {
                    error = true,
                    message = "Şifrə adı göndərilməyib!"
                });
            }

            AppUser appUser = null;
            if (formModel.Username.IsEmail())
            {
                appUser = await userManager.FindByEmailAsync(formModel.Username);
            }
            else
            {
                appUser = await userManager.FindByNameAsync(formModel.Username);
            }

            if (appUser == null)
            {
                return BadRequest(new
                {
                    error = true,
                    message = "İstifadəçi adı və ya şifrə yanlışdır!"
                });
            }

            SignInResult result = await signInManager.CheckPasswordSignInAsync(appUser, formModel.Password, true);

            if (result.IsLockedOut)
            {
                return BadRequest(new
                {
                    error = true,
                    message = "Hesabınız müvəqqəti olaraq bloklanıb!"
                });
            }
            else if (result.IsNotAllowed)
            {
                return BadRequest(new
                {
                    error = true,
                    message = "Hesabınız bloklanıb!"
                });
            }
            else if (result.Succeeded)
            {
                goto stopGenerate;
            }
            else
            {

                return BadRequest(new
                {
                    error = true,
                    message = "Giriş hüququnuz yoxdur!"
                });
            }


        stopGenerate:
            string secret = "@p!-7@bb!TMq.".Encrypt();
            List<Claim> claims = new();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()));
            if (!string.IsNullOrWhiteSpace(appUser.Name) || !string.IsNullOrWhiteSpace(appUser.Surname))
            {
                claims.Add(new Claim("FullName", $"{appUser.Name} {appUser.Surname}"));
            }
            else if (!string.IsNullOrWhiteSpace(appUser.PhoneNumber))
            {
                claims.Add(new Claim("FullName", $"{appUser.PhoneNumber}"));
            }
            else
            {
                claims.Add(new Claim("FullName", $"{appUser.Email}"));
            }
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            string issuer = conf["Jwt:Issuer"];
            string audience = conf.GetValue<string>("Jwt:Audience");

            byte[] buffer = Encoding.UTF8.GetBytes(conf["Jwt:Secret"]);
            var key = new SymmetricSecurityKey(buffer);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expired = DateTime.UtcNow.AddHours(4).AddMinutes(10);

            var tokenBuilder = new JwtSecurityToken(issuer, audience, claims,
                                                    expires: expired,
                                                    signingCredentials: creds);

            string token = new JwtSecurityTokenHandler().WriteToken(tokenBuilder);

            return Ok(new
            {
                error = false,
                message = "Xoş gəlmişsiniz!",
                token,
                expired
            });
        }
    }
}
