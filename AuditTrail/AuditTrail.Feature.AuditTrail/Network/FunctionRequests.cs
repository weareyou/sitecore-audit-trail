using AuditTrail.Feature.AuditTrail.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AuditTrail.Feature.AuditTrail.Network
{
    public static class FunctionRequests
    {
        public static void SendEventData(AuditRecord record)
        {
            string recordJson = JsonConvert.SerializeObject(record);
            PostJson(recordJson);
        }

        public static void SendEventData(UserInteractionRecord record)
        {
            string recordJson = JsonConvert.SerializeObject(record);
            PostJson(recordJson);
        }

        private static void PostJson(string json)
        {
            HttpClient client = new HttpClient();

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync(Properties.Resources.STORE_EVENT_DATA_FUNCTION_URL, content);
        }
        /*
        public async static Task<string> GetItemViewHTML()
        {
            HttpClient client = new HttpClient();

            string page = await client.GetAsync("https://sitecoreaudittrail.blob.core.windows.net/$web/index.html");

        }*/
    }
}