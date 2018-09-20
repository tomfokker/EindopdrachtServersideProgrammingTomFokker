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
    public static class FunctionQueueTrigger
    {
        [FunctionName("FunctionQueueTrigger")]
        public static void Run([QueueTrigger("beerqueue", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");

            QueueMessage queueItem = Newtonsoft.Json.JsonConvert.DeserializeObject<QueueMessage>(myQueueItem);

            OpenWeatherMapAPIClient api = new OpenWeatherMapAPIClient();
            OpenWeatherMapResult weather = api.GetWeather(queueItem.cityName);
            
            // Determine beer weather
            string beerAdvice;

            if (weather != null)
            {
                if ((weather.main.temp - 272.15) < 16)
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

            // Create blob reference 
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("somecontainer");
            container.CreateIfNotExistsAsync();
            container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            
            var blob = container.GetBlockBlobReference(queueItem.blobName);

            // Create error image
            if (weather == null)
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
