using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using AuditTrail.Feature.AuditTrail.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuditTrail.Feature.AuditTrail.AzureFunctions.Routes
{
    public static class GetByItem
    {
        [FunctionName("GetByItem")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item/{itemId}")]HttpRequestMessage req,
            string itemId,

            [CosmosDB(
                databaseName: "audit-trail",
                collectionName: "audit-records",
                ConnectionStringSetting = "COSMOS_CONNECTION_STRING")] DocumentClient client,
            ILogger log)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri("audit-trail", "audit-records");

            var options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = 30 };

            var query = client.CreateDocumentQuery<AuditRecord>(collectionUri, options)
                .Where(a => a.ItemId == itemId)
                .OrderByDescending(q => q.Timestamp)
                .Take(30)
                .AsDocumentQuery();

            var results = new List<AuditRecord>();

            while (query.HasMoreResults)
            {
                foreach (AuditRecord record in await query.ExecuteNextAsync())
                {
                    results.Add(record);
                }
            }

            if (results.Count == 0)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
                {
                    Content = new StringContent("No content found.")
                };
            }
            else
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(results), Encoding.UTF8, "application/json")
                };
            }
        }
    }
}