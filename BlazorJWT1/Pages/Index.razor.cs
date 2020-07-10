using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Models;

namespace BlazorJWT1.Pages
{

    public class LoginBase : ComponentBase
    {




        [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] ILogger<LoginBase> Log { get; set; }
        [Inject] UserManager<ApplicationUser> userManager { get; set; }
        [Inject] SignInManager<ApplicationUser> signInManager { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        [Inject] IDataProtectionProvider dataProtectionProvider { get; set; }

        protected SignInModel signInModel = new SignInModel();
        protected bool showSignInError = false;
        public static AuthenticationState AuState = null;

        protected async Task Login()
        {
            var user = await userManager.FindByEmailAsync(signInModel.Email);

            if (user != null && await userManager.CheckPasswordAsync(user, signInModel.Password))
            {
                showSignInError = false;

                var token = await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "SignIn");

                var data = $"{user.Id}|{token}";

                var parsedQuery = System.Web.HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);

                var returnUrl = parsedQuery["returnUrl"];

                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    data += $"|{returnUrl}";
                }

                var protector = dataProtectionProvider.CreateProtector("SignIn");

                var pdata = protector.Protect(data);

                navigationManager.NavigateTo("/account/login?t=" + pdata, forceLoad: true);
            }
            else
            {
                showSignInError = true;
            }
        }


        protected override async Task OnInitializedAsync()
        {
            AuState = await authenticationStateProvider.GetAuthenticationStateAsync();
            Log.LogInformation($"MainNav.AuthenticationStateProvider.User (OnInitializedAsync) ={AuState.User.Identity.Name}");
        }

        public class SignInModel
        {
            [System.ComponentModel.DataAnnotations.Required]
            public string Email { get; set; }
            [System.ComponentModel.DataAnnotations.Required]
            public string Password { get; set; }
        }

    }
}