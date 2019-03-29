using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SitecoreLogicConnector.Feature.SLC.AzureFunctions.Authentication;

namespace SitecoreLogicConnector.Feature.SLC.AzureFunctions.ItemServiceAPI
{
    public static class ItemServiceCreate
    {
        // todo: create boilerplate
        [FunctionName("ItemServiceCreate")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ItemService")]
            HttpRequestMessage req, TraceWriter log)
        {
            var queryParameters = req.GetQueryNameValuePairs().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var itemPath = queryParameters["ItemPath"];

            if (!req.Headers.TryGetValues("username", out var username))
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized, "Please pass Sitecore username in the header.");
            }

            if (!req.Headers.TryGetValues("password", out var password))
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized, "Please pass Sitecore password in the header.");
            }

            var authCookie = await SitecoreAuth.Login(username.FirstOrDefault(), password.FirstOrDefault());
            var sitecoreJson = await PerformSitecoreRequest(authCookie, itemPath, req.Content);

            var message = req.CreateResponse(HttpStatusCode.OK);
            message.Content = new StringContent(sitecoreJson, Encoding.UTF8, "application/json");

            return message;
        }

        private static async Task<string> PerformSitecoreRequest(Cookie authCookie, string itemPath,
            HttpContent requestBody)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler {CookieContainer = cookies};
            var client = new HttpClient(handler);
            var requestRoute = Environment.GetEnvironmentVariable("SITECORE_BASE_URL", EnvironmentVariableTarget.Process) + "/api/ssc/item/" + itemPath;
            Debug.WriteLine(requestRoute);

            cookies.Add(authCookie);

            var request = new HttpRequestMessage(HttpMethod.Post, requestRoute) {Content = requestBody};
            var response = await client.SendAsync(request);

            var json = "Creation of the object has failed. Check if the given path is valid.";

            // todo: prototype code
            if (response.Headers.TryGetValues("Location", out var location))
            {
                var itemId = location.FirstOrDefault();
                if (itemId != null)
                {
                    itemId = itemId.Split('/').Last();
                    itemId = itemId.Split('?').FirstOrDefault();
                    json = "{ \"ItemID\": \"" + itemId + "\" }";
                }
            }

            return json;
        }
    }
}
