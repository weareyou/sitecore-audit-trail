using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace SitecoreLogicConnector.Feature.SLC.Network
{
    public static class FunctionRequests
    {
        public static void TriggerLogicConnectorEvent(string eventName, Dictionary<string, string> body)
        {
            HttpClient client = new HttpClient();

            string bodyJson = JsonConvert.SerializeObject(body);

            var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            var response = client.PostAsync(Properties.Resources.AzureFunctionsConnectorURL + "/ItemDeleted?" + Properties.Resources.AzureFunctionsConnectorAuthCode, content);
        }
    }
}