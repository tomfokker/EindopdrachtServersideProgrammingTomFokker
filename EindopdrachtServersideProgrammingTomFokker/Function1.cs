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
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            
            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            string lastname = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "lastname", true) == 0)
                .Value;

            OpenWeatherMapAPIClient api = new OpenWeatherMapAPIClient();
            OpenWeatherMapResult weather = api.GetWeather("London", "uk");

            name = weather.main.temp.ToString();

            // Storage acccount
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tomazureteststorage;AccountKey=8M0CNkCnMqzgPcliz3wYaBcR+HF8BXbVb9suJK6z942qNJlrEgUTE2/Yq+/u9BgOCOqu8U13K6+x+NbNimKzyw==;EndpointSuffix=core.windows.net");

            // Blob
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("somecontainer");
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            var blobName = $"{Guid.NewGuid().ToString()}.png";
            

            // Create Queue message to trigger FunctionQueueTrigger
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("beerqueue");
            await queue.CreateIfNotExistsAsync();

            //var message = weather.coord.lon.ToString()+ " " + weather.coord.lat.ToString() + " " + weather.main.temp.ToString()+" "+ weather.wind.speed.ToString();
            var message = blobName;
            await queue.AddMessageAsync(new CloudQueueMessage(message));

            

            // Get blob uri
            string uri = container.StorageUri.PrimaryUri.AbsoluteUri;
            string url = uri + "/" + blobName;

            //WebRequest wr = WebRequest.Create(container.StorageUri.PrimaryUri);


            if (name == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                name = data?.name;
            }

            if (lastname == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                lastname = data?.lastname;
            }

            //return new OkObjectResult(name);

            if (name == null || lastname == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body", "text/plain");
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.OK, "Hello " + url, "text/plain");
            }
            /*
            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body", "text/plain")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name, "text/plain"); 
            */
        }

    }
}
