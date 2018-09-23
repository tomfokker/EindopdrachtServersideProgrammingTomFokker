using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EindopdrachtServersideProgrammingTomFokker
{
    public static class FunctionSecondQueueTrigger
    {
        [FunctionName("FunctionSecondQueueTrigger")]
        public static void Run([QueueTrigger("weatherqueue", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");

            SecondQueueMessage secondQueueItem = Newtonsoft.Json.JsonConvert.DeserializeObject<SecondQueueMessage>(myQueueItem);

            /*
            QueueMessage queueItem = Newtonsoft.Json.JsonConvert.DeserializeObject<QueueMessage>(myQueueItem);

            OpenWeatherMapAPIClient api = new OpenWeatherMapAPIClient();
            OpenWeatherMapResult weather = api.GetWeather(queueItem.cityName);
            */

            // Determine beer weather
            string beerAdvice;

            if (secondQueueItem.weather != null)
            {
                if ((secondQueueItem.weather.main.temp - 272.15) < 16)
                {
                    beerAdvice = "Bier drinken wordt afgeraden.";
                }
                else
                {
                    beerAdvice = "Bier drinken is mogelijk";
                }
            }
            else
            {
                // Error beer advice
                beerAdvice = "Fout: is de plaatsnaam correct?";
            }

            // Get storage acccount
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var storageAccount = CloudStorageAccount.Parse(connectionString + ";EndpointSuffix=core.windows.net");

            // Create blob reference, permissions can remain private because a SAS token is used to retrieve the blob
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("somecontainer");
            container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(secondQueueItem.blobName);

            // Create error image
            if (secondQueueItem.weather == null)
            {
                Bitmap errorImage = new Bitmap(600, 600);
                MemoryStream errorMemoryStream = new MemoryStream();
                errorImage.Save(errorMemoryStream, ImageFormat.Png);
                ImageTextDrawer errorTextDrawer = new ImageTextDrawer();
                blob.UploadFromStreamAsync(errorTextDrawer.DrawTextOnImage(errorMemoryStream, beerAdvice, "-", "-"));
                return;
            }

            // Get image from Azure maps
            log.Info($"C# Queue trigger function processed: azure maps");                        
            AzureMapsRenderAPIClient azureMapsClient = new AzureMapsRenderAPIClient();
            MemoryStream memoryStream = azureMapsClient.GetMap(secondQueueItem.weather.coord.lon, secondQueueItem.weather.coord.lat);

            // Draw text on image
            log.Info($"C# Queue trigger function processed: weer variabelen");
            string temperature = (secondQueueItem.weather.main.temp - 272.15).ToString();
            string windspeed = secondQueueItem.weather.wind.speed.ToString();
            
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
