using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuTestMicroservice.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace BlazorJWT1.Pages
{



    public class FetchDataBase : ComponentBase
    {

        [Inject] HttpClient httpClient { get; set; }
        [Inject] IHttpContextAccessor http { get; set; }
        [Inject] IConfiguration Configuration { get; set; }

        public WeatherForecast[] forecasts;
        public string Lerr1;

        protected override async Task OnInitializedAsync()
        {
            HttpResponseMessage response;
            try
            {
                var AuCookies = http.HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];
                string JWTtoken;
                if (AuCookies != null)
                {
                    httpClient.DefaultRequestHeaders.Add("Cookie", ".AspNetCore.Identity.Application=" + AuCookies);
                    JWTtoken = httpClient.GetStringAsync($"{http.HttpContext.Request.Scheme}://{http.HttpContext.Request.Host}/account/gettoken").Result.Replace("\"", "");
                    if (JWTtoken != null)
                    {
                        httpClient.DefaultRequestHeaders.Remove("Cookie");

                        //redifinition authentication from site cookie to JWT for microservices

                        httpClient.DefaultRequestHeaders.Clear();
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {JWTtoken }");
                        response = httpClient.GetAsync(Configuration["MicriservicesUrl"]).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string dataObject = response.Content.ReadAsStringAsync().Result;
                            forecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(dataObject);
                            StateHasChanged();
                            //forecasts = await ForecastService.GetForecastAsync(DateTime.Now); -- old local call microservices
                        }
                        else
                            Lerr1 = response.StatusCode.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Lerr1 = e.Message;
            }
        }

    }


}
