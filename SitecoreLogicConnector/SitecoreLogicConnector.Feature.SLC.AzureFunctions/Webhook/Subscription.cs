using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace SitecoreLogicConnector.Feature.SLC.AzureFunctions.WebHook
{
    public static class Subscription
    {
        [FunctionName("Subscription")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", "delete", Route = "Subscription/{eventName}")]
            HttpRequestMessage req, string eventName, TraceWriter log)
        {
            log.Info("Subscription request received");
            // check whether eventName is valid? (enum?)

            // get the cloud table
            var storageAccount = CloudStorageAccount.Parse(
                Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING",
                    EnvironmentVariableTarget.Process));

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(Environment.GetEnvironmentVariable("STORAGE_CALLBACK_TABLE",
                EnvironmentVariableTarget.Process));

            // check http method
            if (req.Method == HttpMethod.Post)
                return await Subscribe(req, table, eventName);

            else if (req.Method == HttpMethod.Delete)
                return await Unsubscribe(req, table);

            else
                return req.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong with the request.");
        }

        private static async Task<HttpResponseMessage> Subscribe(HttpRequestMessage req, CloudTable table,
            string eventName)
        {
            var jsonContent = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);
            string callbackUrl = data?.CallbackUrl;


            // create subscription record
            var subscription = new WebHookSubscription()
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = eventName,
                CallbackUrl = callbackUrl
            };

            // save in table
            var insert = TableOperation.Insert(subscription);
            await table.ExecuteAsync(insert);

            // return url in "Location" header
            var response = req.CreateResponse(HttpStatusCode.Created, "Web hook subscription successfully created");
            response.Headers.Add("Location",
                Environment.GetEnvironmentVariable("FUNCTION_APP_DOMAIN", EnvironmentVariableTarget.Process)
                + "/api/Subscription/DELETE?code="
                + Environment.GetEnvironmentVariable("SUBSCRIPTION_ROUTE_CODE", EnvironmentVariableTarget.Process)  
                + "&id=" 
                + subscription.Id 
                + "&event=" 
                + eventName);
            return response;
        }

        private static async Task<HttpResponseMessage> Unsubscribe(HttpRequestMessage req, CloudTable table)
        {
            var id = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "id", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;

            var eventName = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "event", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;

            var toDelete = new WebHookSubscription()
            {
                Id = id,
                PartitionKey = eventName,
                ETag = "*"
            };

            var delete = TableOperation.Delete(toDelete);
            await table.ExecuteAsync(delete);

            return req.CreateResponse(HttpStatusCode.OK, "Web hook subscription successfully deleted");
        }
    }
}
