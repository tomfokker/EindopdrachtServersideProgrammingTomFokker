using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace EindopdrachtServersideProgrammingTomFokker
{
    public static class BeerRequest
    {
        [FunctionName("BeerRequest")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            
            // parse query parameter
            string cityName = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "city", true) == 0)
                .Value;

            string countryCode = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "countrycode", true) == 0)
                .Value;

            if (cityName == null || countryCode == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a city and a countrycode on the query string or in the request body", "text/plain");
            }

            log.Info("C# connectionstring.");
            // Get storage acccount
            //var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tomazureteststorage;AccountKey=q0DOCUvlKZbogKNVkkZTiASchMmI3jh8PdYjs8+HOqsopXyHEAudCg+iwLz7HEOvTWpMVwrhGqsY0AXeVtHBkQ==;EndpointSuffix=core.windows.net");
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var storageAccount = CloudStorageAccount.Parse(connectionString + ";EndpointSuffix=core.windows.net");
            log.Info("C# connectionstring.");
            // Create blobname
            var blobName = $"{Guid.NewGuid().ToString()}.png";            

            // Get queue reference
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("beerqueue");
            await queue.CreateIfNotExistsAsync();

            // Create Queue message to trigger FunctionQueueTrigger
            QueueMessage queueMessage = new QueueMessage();
            queueMessage.cityName = cityName;
            queueMessage.countryCode = countryCode;
            queueMessage.blobName = blobName;
            string message = Newtonsoft.Json.JsonConvert.SerializeObject(queueMessage);

            // Add message to queue
            await queue.AddMessageAsync(new CloudQueueMessage(message));

            if (blobName == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Image could not be created", "text/plain");
            }
            else
            {
                Uri currentUrl = req.RequestUri;
                string absoluteUrl = currentUrl.AbsoluteUri;
                string urlWithoutQuery = absoluteUrl.Split('?')[0];
                string beerReportUrl = urlWithoutQuery.Replace("beerrequest", "beerreport?imagename=" + blobName);
                log.Info(beerReportUrl);

                var response = req.CreateResponse(HttpStatusCode.Moved);
                response.Headers.Location = new Uri(beerReportUrl);
                return response;
                
            }
        }

    }
}
