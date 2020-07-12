using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuTestMicroservice.Data
{
    public class WeatherForecast : IWeatherForecast
    {
        public DateTime Date { get; set; }
        public string Summary { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF { get; }
    };
}
