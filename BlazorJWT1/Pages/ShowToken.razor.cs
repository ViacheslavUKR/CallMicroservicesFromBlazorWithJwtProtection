using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorJWT1.Pages
{
    public class ShowTokenBase : ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; }
        [Parameter] public MarkupString Bearer { get; set; }
        protected void GetToken()
        {
            navigationManager.NavigateTo("/account/GetTokenForShow", true);
        }

        protected override void OnInitialized()
        {
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("t", out var bearer))
            {
                if (!StringValues.IsNullOrEmpty(bearer))
                {
                    Bearer = (MarkupString)FormatJWT(bearer);
                }
            }
            base.OnInitialized();
        }

        string FormatJWT(string Token) {
            var t = new JwtSecurityTokenHandler().ReadToken(Token);
            var tf = JsonConvert.SerializeObject(t, Formatting.Indented);
            return tf.Replace("\r\n","<br>");
        }
        
   }
}
