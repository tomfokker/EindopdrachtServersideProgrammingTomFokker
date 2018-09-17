using System;
using System.IO;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

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

            // Determine beer weather
            string beerAdvice;
            if ((weather.main.temp - 272.15) < 16 || weather.wind.speed > 7.9)
            {
                beerAdvice = "Bier drinken wordt afgeraden.";
            }
            else
            {
                beerAdvice = "Bier drinken is mogelijk";
            }

            // Get storage acccount
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tomazureteststorage;AccountKey=8M0CNkCnMqzgPcliz3wYaBcR+HF8BXbVb9suJK6z942qNJlrEgUTE2/Yq+/u9BgOCOqu8U13K6+x+NbNimKzyw==;EndpointSuffix=core.windows.net");

            // Create blob reference 
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("somecontainer");
            container.CreateIfNotExistsAsync();
            container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            
            var blob = container.GetBlockBlobReference(queueItem.blobName);

            log.Info($"C# Queue trigger function processed: azure maps");                        
            AzureMapsRenderAPIClient azureMapsClient = new AzureMapsRenderAPIClient();
            MemoryStream memoryStream = azureMapsClient.GetMap(weather.coord.lon, weather.coord.lat);

            // Draw text on image
            log.Info($"C# Queue trigger function processed: weer variabelen");
            string temperature = (weather.main.temp - 272.15).ToString();
            string windspeed = weather.wind.speed.ToString();
            
            log.Info($"C# Queue trigger function processed: tekst tekenen");  
            MemoryStream outMemoryStream = new MemoryStream();
            ImageTextDrawer textDrawer = new ImageTextDrawer();
            outMemoryStream = textDrawer.DrawTextOnImage(memoryStream, beerAdvice, temperature, windspeed);


            log.Info($"C# Queue trigger function processed: vlak voor upload");
            if (!(outMemoryStream is null))
            {
                // Upload image
                blob.UploadFromStreamAsync(outMemoryStream);
            }
            else
            {
                log.Info($"C# Queue trigger function processed: upload mislukt");
            }
            
        }
    }
}
