using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using System.Net.Http;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;
using AuditTrail.Feature.AuditTrail.Models;
using System.Text;
using System.Diagnostics;

namespace AuditTrail.Feature.AuditTrail.AzureFunctions.Routes
{
    // Standard API
    // Possible routes:
    // GET /data                                  Must have a filter in the body. Returns filtered results.
    // GET /item/{itemId}                         all recorded changes of item
    // GET /item/{itemId}/meta                    total change count, total authors, last change date, last change author
    // GET /author/{author}                       all changes made by author (ID or name)
    // GET /event/eventName

    // Filter:
    // Startdate
    // Enddate
    // Author(s)




    public static class AuditTrailData
    {
        [FunctionName("AuditTrailData")]
        public async static Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "recent")]HttpRequestMessage req,

            [CosmosDB(
                databaseName: "audit-trail",
                collectionName: "audit-records",
                ConnectionStringSetting = "COSMOS_CONNECTION_STRING")] DocumentClient client,
            ILogger log)
        {
            // query params for pagination
            var queryParamaters = req.RequestUri.ParseQueryString();
            /*
            int count = 20;
            if (queryParamaters["count"] != null)
                count = Convert.ToInt32(queryParamaters["count"]);
                */
            string requestToken = "";
            if (queryParamaters["token"] != null)
                requestToken = queryParamaters["token"];

            int maxItemCount = 30;

            // if a token is provided, return a continuation of a previous search
            var options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = maxItemCount };
            if (requestToken != "")
                options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = maxItemCount, RequestContinuation = requestToken };


            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("audit-trail", "audit-records");



            IDocumentQuery<AuditRecord> query = client.CreateDocumentQuery<AuditRecord>(collectionUri, options)
                .OrderByDescending(q => q.Timestamp)
                .Take(30)
                .AsDocumentQuery();

            List<AuditRecord> records = new List<AuditRecord>();

            string continuationToken = "";

            // apparently MaxItemCount in the options does not stop the query from producing more results, so we check the page limit manually.
            while (query.HasMoreResults && records.Count < maxItemCount+1)
            {
                var results = await query.ExecuteNextAsync<AuditRecord>();

                foreach (AuditRecord record in results)
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

/*
dynamic data = await req.Content.ReadAsAsync<object>();
string eventName = data?.eventName;
string tableValue = data?.tableValue;
string tableFilterType = data?.tableFilterType;*/
