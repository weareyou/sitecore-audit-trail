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

namespace SitecoreLogicConnector.Feature.SLC.AzureFunctions.WebHook
{
    public static class SitecoreEventHook
    {
        [FunctionName("SitecoreEventHook")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "SitecoreEventHook/{eventName}")]HttpRequestMessage req, string eventName, TraceWriter log)
        {
            log.Info($"SitecoreEventHook web hook was triggered!");

            // get the cloud table
            var storageAccount = CloudStorageAccount.Parse(
                Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING", EnvironmentVariableTarget.Process));

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(Environment.GetEnvironmentVariable("STORAGE_CALLBACK_TABLE", EnvironmentVariableTarget.Process));

            // check JSON contents if necessary
            if (eventName != null)
                return await PerformCallbacks(req, table, eventName);
            else
                return req.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong with the request.");
        }

        private static async Task<HttpResponseMessage> PerformCallbacks(HttpRequestMessage req, CloudTable table, string eventName)
        {
            IEnumerable<WebHookSubscription> query = (from sub in table.CreateQuery<WebHookSubscription>() where sub.PartitionKey == eventName select sub);

            var client = new HttpClient();

            var json = await req.Content.ReadAsStringAsync();

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            foreach (var hook in query)
            {
                await client.PostAsync(hook.CallbackUrl, content); // should this be async?
            }

            return req.CreateResponse(HttpStatusCode.OK, "Web hook callbacks successfully called.");
        }

    }
}
