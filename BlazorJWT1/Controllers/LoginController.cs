using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlazorJWT1.Pages;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Models;
using static BlazorJWT1.Pages.LoginBase;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
//using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace BlazorJWT1.Controllers
{
    [ApiController]
    [Authorize]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;
        private readonly IDataProtector dataProtector;

        public LoginController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, IWebHostEnvironment _hostingEnvironment, IConfiguration _configuration, IDataProtectionProvider dataProtectionProvider)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            hostingEnvironment = _hostingEnvironment;
            configuration = _configuration;
            dataProtector = dataProtectionProvider.CreateProtector("SignIn");
        }

        [HttpGet("account/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string t)
        {
            if (!string.IsNullOrEmpty(t))
            {
                var data = dataProtector.Unprotect(t);

                var parts = data.Split('|');

                var identityUser = await userManager.FindByIdAsync(parts[0]);

                var isTokenValid = await userManager.VerifyUserTokenAsync(identityUser, TokenOptions.DefaultProvider, "SignIn", parts[1]);

                if (isTokenValid)
                {
                    await signInManager.SignInAsync(identityUser, true);
                    if (parts.Length == 3 && Url.IsLocalUrl(parts[2]))
                    {
                        return Redirect(parts[2]);
                    }
                    return Redirect("/");
                }
                else
                {
                    return Unauthorized("STOP!");
                }
            }
            else
            {
                return Unauthorized("STOP!");
            }
        }

        [HttpGet("account/gettokenforshow")]
        public IActionResult GetTokenForShow()
        {
            var identityUser = userManager.GetUserAsync(HttpContext.User).Result;
            if (identityUser != null)
            {
                string Bearer = JsonWebToken(identityUser);
                return Redirect("/showtoken?t=" + Bearer);
            }

            return Unauthorized("STOP!");
        }

        [HttpGet("account/gettoken")]
        public string GetToken()
        {
            var identityUser = userManager.GetUserAsync(HttpContext.User).Result;
            if (identityUser != null)
            {
                return JsonWebToken(identityUser);
            }
            return "";
        }

        [HttpPost("account/checkuser")]
        public ApplicationUser CheckUser(SignInModel user)
        {
            var User1 = userManager.FindByEmailAsync(user.Email).Result;

            if (User1 != null && userManager.CheckPasswordAsync(User1, user.Password).Result)
            {
                return User1;
            }
            else return null;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("account/checkadmin")]
        public IActionResult CheckAdmin([FromBody] string value)
        {
            try
            {
                return Ok(value);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        private string JsonWebToken(ApplicationUser currentUser)
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credintals = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, currentUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role,"None")
            };

            var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    audience: configuration["Jwt:Issuer"],
                    claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credintals
                );
            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        }

        [Authorize]
        [HttpGet("account/signout")]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();

            return Redirect("/");
        }
    }
}
//Dictionary<string, string> header = new Dictionary<string, string>();
//header.Add("Authorization", "Bearer " + Bearer);