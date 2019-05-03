using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using AuditTrail.Feature.AuditTrail.Models;

namespace AuditTrail.Feature.AuditTrail.AzureFunctions
{
    public static class StoreAuditRecord
    {
        [FunctionName("StoreAuditRecord")]
        public static void Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "StoreEventData")]HttpRequestMessage req,
            [CosmosDB(
                databaseName: "audit-trail",
                collectionName: "audit-records",
                ConnectionStringSetting = "COSMOS_CONNECTION_STRING",
                CreateIfNotExists = true,
                CollectionThroughput = 400)] out dynamic document,
            ILogger log)
        {
            document = JsonConvert.DeserializeObject<AuditRecord>(req.Content.ReadAsStringAsync().Result);

            log.LogInformation("Event data stored to DB.");
        }
    }
}
