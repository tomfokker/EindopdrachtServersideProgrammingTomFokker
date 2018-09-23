using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

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

            // Get storage acccount
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var storageAccount = CloudStorageAccount.Parse(connectionString + ";EndpointSuffix=core.windows.net");
            log.Info("C# connectionstring.");

            // Get queue reference
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("weatherqueue");
            queue.CreateIfNotExistsAsync();

            // Create Queue message to trigger FunctionSecondQueueTrigger
            SecondQueueMessage secondQueueMessage = new SecondQueueMessage();
            secondQueueMessage.blobName = queueItem.blobName;
            secondQueueMessage.weather = weather;

            string message = Newtonsoft.Json.JsonConvert.SerializeObject(secondQueueMessage);

            // Add message to queue
            queue.AddMessageAsync(new CloudQueueMessage(message));

        }
    }
}
