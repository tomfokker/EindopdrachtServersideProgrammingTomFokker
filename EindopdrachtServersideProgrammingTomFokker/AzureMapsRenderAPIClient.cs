using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace EindopdrachtServersideProgrammingTomFokker
{
    class AzureMapsRenderAPIClient
    {
        private const string url = "https://atlas.microsoft.com/map/static/png";
        private string apiKey = Environment.GetEnvironmentVariable("AzureMapsApiKey");

        private MemoryStream RunRequest(string queryParameters)
        {
            WebClient webClient = new WebClient();
            MemoryStream memoryStream;

            try
            {
                byte[] bytes = webClient.DownloadData(url+ queryParameters);
                memoryStream = new MemoryStream(bytes);
            }
            catch (System.Exception e)
            {
                // memoryStream can't be empty
                //memoryStream = new MemoryStream();
                return null;
            }
            finally
            {
                webClient.Dispose();
            }

            return memoryStream;
        }

        public MemoryStream GetMap(double lon, double lat)
        {
            int zoom = 12;
            string layer = "basic";
            string queryParameters = "?subscription-key=" + this.apiKey + "&api-version=1.0&style=main&layer=" + layer + "&zoom=" + zoom.ToString() + "&center=" + lon.ToString() + "," + lat.ToString();
            return this.RunRequest(queryParameters);
        }
    }
}
