using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using System.Net.Http;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;
using AuditTrail.Feature.AuditTrail.Models;
using System.Text;

namespace AuditTrail.Feature.AuditTrail.AzureFunctions.Routes
{
    public static class AuditTrailData
    {
        [FunctionName("AuditTrailData")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "recent")]HttpRequestMessage req,

            [CosmosDB(
                databaseName: "audit-trail",
                collectionName: "audit-records",
                ConnectionStringSetting = "COSMOS_CONNECTION_STRING")] DocumentClient client,
            ILogger log)
        {
            // query params for pagination
            var queryParameters = req.RequestUri.ParseQueryString();
            var requestToken = string.Empty;
            if (queryParameters["token"] != null)
                requestToken = queryParameters["token"];

            const int maxItemCount = 30;

            // if a token is provided, return a continuation of a previous search
            var options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = maxItemCount };
            if (requestToken != "")
                options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = maxItemCount, RequestContinuation = requestToken };

            var collectionUri = UriFactory.CreateDocumentCollectionUri("audit-trail", "audit-records");

            var query = client.CreateDocumentQuery<AuditRecord>(collectionUri, options)
                .OrderByDescending(q => q.Timestamp)
                .Take(30)
                .AsDocumentQuery();

            var records = new List<AuditRecord>();

            var continuationToken = string.Empty;

            // apparently the MaxItemCount option does not stop the query from producing more results, so we check the page limit manually
            while (query.HasMoreResults && records.Count < maxItemCount+1)
            {
                var results = await query.ExecuteNextAsync<AuditRecord>();

                foreach (var record in results)
                {
                    records.Add(record);
                }

                continuationToken = results.ResponseContinuation;
            }
            
            if (records.Count == 0)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
                {
                    Content = new StringContent("No content found.")
                };
            }
            else
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(records, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json")
                };

                if (continuationToken != "")
                    response.Headers.Add("continuationToken", continuationToken);

                return response;
            }
        }
    }
}
