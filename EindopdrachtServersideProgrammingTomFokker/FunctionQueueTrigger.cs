using System;
using System.IO;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;

namespace EindopdrachtServersideProgrammingTomFokker
{
    public static class FunctionQueueTrigger
    {
        [FunctionName("FunctionQueueTrigger")]
        public static void Run([QueueTrigger("beerqueue", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");

            QueueMessage queueItem = Newtonsoft.Json.JsonConvert.DeserializeObject<QueueMessage>(myQueueItem);

            OpenWeatherMapAPIClient api = new OpenWeatherMapAPIClient();
            OpenWeatherMapResult weather = api.GetWeather(queueItem.cityName, queueItem.countryCode);
            string temp = weather.main.temp.ToString();

            // Storage acccount
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tomazureteststorage;AccountKey=8M0CNkCnMqzgPcliz3wYaBcR+HF8BXbVb9suJK6z942qNJlrEgUTE2/Yq+/u9BgOCOqu8U13K6+x+NbNimKzyw==;EndpointSuffix=core.windows.net");

            // Blob
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("somecontainer");

            var blob = container.GetBlockBlobReference(queueItem.blobName);


            log.Info($"C# Queue trigger function processed: vlak voor memory stream");
                        
            AzureMapsRenderAPIClient azureMapsClient = new AzureMapsRenderAPIClient();
            MemoryStream memoryStream = azureMapsClient.GetMap(weather.coord.lon, weather.coord.lat);
            
            log.Info($"C# Queue trigger function processed: vlak voor upload");
            if (!(memoryStream is null))
            {
                blob.UploadFromStreamAsync(memoryStream);
            }
            else
            {
                log.Info($"C# Queue trigger function processed: upload mislukt");
            }
            
        }
    }
}
