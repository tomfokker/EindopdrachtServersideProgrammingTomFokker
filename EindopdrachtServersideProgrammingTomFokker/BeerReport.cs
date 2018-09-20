using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace EindopdrachtServersideProgrammingTomFokker
{
    public static class BeerReport
    {
        [FunctionName("BeerReport")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            
            // parse query parameter
            string imageName = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "imagename", true) == 0)
                .Value;

            if (imageName == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass an image name on the query string or in the request body", "text/plain");
            }

            // Storage acccount
            //var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tomazureteststorage;AccountKey=q0DOCUvlKZbogKNVkkZTiASchMmI3jh8PdYjs8+HOqsopXyHEAudCg+iwLz7HEOvTWpMVwrhGqsY0AXeVtHBkQ==;EndpointSuffix=core.windows.net");
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var storageAccount = CloudStorageAccount.Parse(connectionString + ";EndpointSuffix=core.windows.net");

            // Blob
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("somecontainer");
            
            // Get blob uri
            string uri = container.StorageUri.PrimaryUri.AbsoluteUri;
            string url = uri + "/" + imageName;
            
            if (imageName == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                imageName = data?.name;
            }

            log.Info("Voor image is null controle");
            if (imageName == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body", "text/plain");
            }
            else
            {
                log.Info(url);

                string html = "<html><body><img src=\"" + url + "\" alt=\"Refresh om bier rapport te zien\"></body></html>";

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(html);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }
            
        }
    }
}
