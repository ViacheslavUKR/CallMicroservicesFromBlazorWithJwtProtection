using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Models;

namespace BlazorJWT1.Pages
{
    public class RegisterBase : ComponentBase
    {

        [Inject] UserManager<ApplicationUser> userManager { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }


        public RegisterAccountModel registerAccountModel = new RegisterAccountModel();
        public static string Lerr1="";

        protected async Task RegisterUser()
        {

            var result = await userManager.CreateAsync(new ApplicationUser { UserName = registerAccountModel.Email, Email = registerAccountModel.Email, EmailConfirmed = true }, registerAccountModel.Password);

            if (result.Succeeded)
            {
                navigationManager.NavigateTo("/Login");
                return;
            }
            else {
                if (result.Errors.Count() > 0) {
                    Lerr1 = result.Errors.ToList()[0].Description;
                }
            }
        }

        public class RegisterAccountModel
        {
            [Required]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
            [Required]
            [Compare(nameof(Password))]
            public string ConfirmPassword { get; set; }
        }
    }
}
