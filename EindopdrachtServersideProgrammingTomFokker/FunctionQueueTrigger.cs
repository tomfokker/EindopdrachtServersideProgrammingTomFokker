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

            // Storage acccount
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tomazureteststorage;AccountKey=8M0CNkCnMqzgPcliz3wYaBcR+HF8BXbVb9suJK6z942qNJlrEgUTE2/Yq+/u9BgOCOqu8U13K6+x+NbNimKzyw==;EndpointSuffix=core.windows.net");

            // Blob
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("somecontainer");

            var blob = container.GetBlockBlobReference(myQueueItem);


            log.Info($"C# Queue trigger function processed: vlak voor memory stream");
            /*
            WebClient webClient = new WebClient();
            MemoryStream memoryStream;

            try
            {
                byte[] bytes = webClient.DownloadData("https://i.ytimg.com/vi/Wkk5xn5db7E/maxresdefault.jpg");
                memoryStream = new MemoryStream(bytes);
            }
            catch (System.Exception e)
            {
                // memoryStream can't be empty
                memoryStream = new MemoryStream();
            }
            finally
            {
                webClient.Dispose();
            }
            */
            
            AzureMapsRenderAPIClient azureMapsClient = new AzureMapsRenderAPIClient();
            MemoryStream memoryStream = azureMapsClient.GetMap(-0.13, 51.51);

            //log.Info($"C# Queue trigger function processed: vlak voor stream");
            //var fileStream = File.OpenRead("trash.jpg");

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
