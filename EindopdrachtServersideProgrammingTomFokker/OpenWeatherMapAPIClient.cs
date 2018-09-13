using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace EindopdrachtServersideProgrammingTomFokker
{
    public class OpenWeatherMapAPIClient
    {
        private string url = "http://api.openweathermap.org/data/2.5/weather";
        private string apiKey = "c589371603d5751adb5c198c4db9c22e";
        private string defaultCountryCode = "nl";        

        private string CreateRequest(string queryParameters)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            return "";
        }

        public string GetWeather(string cityName, string countryCode)
        {
            string queryParameters = "?q=" + cityName + "," + countryCode + "&appid=" + apiKey;
            return this.CreateRequest(queryParameters);
        }

        public string GetWeather(string zipCode, string countryCode, bool zip)
        {
            string queryParameters = "?zip=" + zipCode + "," + countryCode + "&appid=" + apiKey;
            return this.CreateRequest(queryParameters);
        }

        public string GetWeather(string cityName)
        {
            return this.GetWeather(cityName, this.defaultCountryCode);
        }

        public string GetWeather(string zipCode, bool zip)
        {
            return this.GetWeather(zipCode, this.defaultCountryCode, zip);
        }
    }
}
