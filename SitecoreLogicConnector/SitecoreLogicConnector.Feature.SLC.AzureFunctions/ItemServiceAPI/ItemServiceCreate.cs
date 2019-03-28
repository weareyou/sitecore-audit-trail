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
        // todo: boilerplate in ItemService klassen wegwerken
        [FunctionName("ItemServiceCreate")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ItemService")]
            HttpRequestMessage req, TraceWriter log)
        {
            Dictionary<string, string> queryParameters = ParseQueryParameters(req.GetQueryNameValuePairs());

            string itemPath = queryParameters["ItemPath"];

            IEnumerable<string> username;
            IEnumerable<string> password;
            if (!req.Headers.TryGetValues("username", out username))
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized, "Please pass Sitecore username in the header.");
            }

            if (!req.Headers.TryGetValues("password", out password))
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized, "Please pass Sitecore password in the header.");
            }

            Cookie authCookie = await SitecoreAuth.Login(username.FirstOrDefault(), password.FirstOrDefault());
            string sitecoreJson = await PerformSitecoreRequest(authCookie, itemPath, req.Content);

            HttpResponseMessage message = req.CreateResponse(HttpStatusCode.OK);
            message.Content = new StringContent(sitecoreJson, Encoding.UTF8, "application/json");

            return message;
        }

        private static async Task<string> PerformSitecoreRequest(Cookie authCookie, string itemPath,
            HttpContent requestBody)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler {CookieContainer = cookies};
            HttpClient client = new HttpClient(handler);
            string requestRoute =
                Environment.GetEnvironmentVariable("SITECORE_BASE_URL", EnvironmentVariableTarget.Process) + "/api/ssc/item/" +
                itemPath;
            Debug.WriteLine(requestRoute);

            cookies.Add(authCookie);

            var request = new HttpRequestMessage(HttpMethod.Post, requestRoute);
            request.Content = requestBody;
            var response = await client.SendAsync(request);

            string json = "Creation of the object has failed. Check if the given path is valid.";

            // todo: dit kan netter
            IEnumerable<string> location;
            if (response.Headers.TryGetValues("Location", out location))
            {
                string itemId = location.FirstOrDefault();
                itemId = itemId.Split('/').Last();
                itemId = itemId.Split('?').FirstOrDefault();
                json = "{ \"ItemID\": \"" + itemId + "\" }";
            }

            return json;
        }

        private static Dictionary<string, string> ParseQueryParameters(
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                queryParams.Add(kvp.Key, kvp.Value);
            }

            return queryParams;
        }
    }
}
