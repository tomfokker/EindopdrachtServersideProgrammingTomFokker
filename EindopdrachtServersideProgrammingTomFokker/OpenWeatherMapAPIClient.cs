using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EindopdrachtServersideProgrammingTomFokker
{
    public class OpenWeatherMapAPIClient
    {
        private const string url = "http://api.openweathermap.org/data/2.5/weather";
        private string apiKey = Environment.GetEnvironmentVariable("OpenWeatherApiKey");   

        private OpenWeatherMapResult RunRequest(string queryParameters)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(queryParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<OpenWeatherMapResult>().Result;
            }
            else
            {
                return null;
            }
        }

        public OpenWeatherMapResult GetWeather(string cityName)
        {
            string queryParameters = "?q=" + cityName + "&appid=" + this.apiKey;
            return this.RunRequest(queryParameters);
        }
    }
}
