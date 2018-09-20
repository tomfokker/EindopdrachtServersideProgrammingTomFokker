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
        private const string defaultCountryCode = "nl";        

        private OpenWeatherMapResult RunRequest(string queryParameters)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(queryParameters).Result;

            OpenWeatherMapResult weather = new OpenWeatherMapResult();

            if (response.IsSuccessStatusCode)
            {
                weather = response.Content.ReadAsAsync<OpenWeatherMapResult>().Result;                
            }

            return weather;
        }

        public OpenWeatherMapResult GetWeather(string cityName, string countryCode)
        {
            string queryParameters = "?q=" + cityName + "," + countryCode + "&appid=" + this.apiKey;
            return this.RunRequest(queryParameters);
        }

        public OpenWeatherMapResult GetWeather(string zipCode, string countryCode, bool zip)
        {
            string queryParameters = "?zip=" + zipCode + "," + countryCode + "&appid=" + this.apiKey;
            return this.RunRequest(queryParameters);
        }

        public OpenWeatherMapResult GetWeather(string cityName)
        {
            return this.GetWeather(cityName, defaultCountryCode);
        }

        public OpenWeatherMapResult GetWeather(string zipCode, bool zip)
        {
            return this.GetWeather(zipCode, defaultCountryCode, zip);
        }
    }
}
