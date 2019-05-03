using System;
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "recent/{pageSize?}/{token?}")]HttpRequestMessage req,
            int? pageSize,
            string token,

            [CosmosDB(
                databaseName: "audit-trail",
                collectionName: "audit-records",
                ConnectionStringSetting = "COSMOS_CONNECTION_STRING")] DocumentClient client,
            ILogger log)
        {
            // if a token is provided, return a continuation of a previous search
            var collectionUri = UriFactory.CreateDocumentCollectionUri("audit-trail", "audit-records");

            var options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = pageSize };
            var query = client.CreateDocumentQuery<AuditRecord>(collectionUri, options)
                .OrderByDescending(q => q.Timestamp)
                .AsDocumentQuery();

            if (token != null)
            {
                options = new FeedOptions {EnableCrossPartitionQuery = true, MaxItemCount = pageSize, RequestContinuation = token};

                query = client.CreateDocumentQuery<AuditRecord>(collectionUri, options)
                    .AsDocumentQuery();

            }

            var records = new List<AuditRecord>();

            var results = await query.ExecuteNextAsync<AuditRecord>();

            foreach (var record in results)
            {
                records.Add(record);
            }

            var continuationToken = results.ResponseContinuation;

            
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
                {
                    response.Headers.Add("continuationToken", continuationToken);
                    response.Headers.Add("Access-Control-Allow-Headers", "*");
                    response.Headers.Add("Access-Control-Expose-Headers", "*");
                }

                return response;
            }
        }
    }
}
