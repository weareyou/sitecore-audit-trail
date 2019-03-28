using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace SitecoreLogicConnector.Feature.SLC.AzureFunctions.Webhook
{
    public static class SitecoreEventHook
    {
        [FunctionName("SitecoreEventHook")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "SitecoreEventHook/{eventName}")]HttpRequestMessage req, string eventName, TraceWriter log)
        {
            log.Info($"SitecoreEventHook webhook was triggered!");

            // Get the cloud table
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING", EnvironmentVariableTarget.Process));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(Environment.GetEnvironmentVariable("STORAGE_CALLBACK_TABLE", EnvironmentVariableTarget.Process));

            // Check JSON contents if necessary
            if (eventName != null)
                return await PerformCallbacks(req, table, eventName);
            else
                return req.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong with the request.");
        }

        private static async Task<HttpResponseMessage> PerformCallbacks(HttpRequestMessage req, CloudTable table, string eventName)
        {
            IEnumerable<WebhookSubscription> query = (from sub in table.CreateQuery<WebhookSubscription>() where sub.PartitionKey == eventName select sub);

            HttpClient client = new HttpClient();

            string json = await req.Content.ReadAsStringAsync();

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            foreach (WebhookSubscription hook in query)
            {
                await client.PostAsync(hook.CallbackUrl, content); // maybe not async later?
            }

            return req.CreateResponse(HttpStatusCode.OK, "Webhook callbacks successfully called.");
        }

    }
}
