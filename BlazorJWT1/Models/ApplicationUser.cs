using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Models
{
    public class ApplicationUser : IdentityUser
    {
        public string GoogleAuthenticatorSecretKey { get; set; }
        public string IPAddress { get; set; }

    }
}