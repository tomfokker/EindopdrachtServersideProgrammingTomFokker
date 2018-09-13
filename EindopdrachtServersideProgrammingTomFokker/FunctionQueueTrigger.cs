using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace EindopdrachtServersideProgrammingTomFokker
{
    public static class FunctionQueueTrigger
    {
        [FunctionName("FunctionQueueTrigger")]
        public static void Run([QueueTrigger("beerqueue", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
