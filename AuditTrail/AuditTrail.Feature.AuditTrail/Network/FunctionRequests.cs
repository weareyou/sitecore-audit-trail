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

        
        public async static Task<string> GetItemViewHTML()
        {
            HttpClient client = new HttpClient();

            string webpath = "https://sitecoreaudittrail.blob.core.windows.net/$web/";

            var page = await client.GetAsync("https://sitecoreaudittrail.blob.core.windows.net/$web/index.html");

            string html = await page.Content.ReadAsStringAsync();

            html = html.Replace("href=/", "href=" + webpath);
            html = html.Replace("src=/", "src=" + webpath);

            return html;
        }

        public async static Task<List<AuditRecord>> GetItemHistory(string itemId)
        {
            HttpClient client = new HttpClient();

            List<AuditRecord> records = new List<AuditRecord>();
            string route = "http://audit-trail.azurewebsites.net/api/item/" + itemId + "?code=wV12VHq7F0ACsu0FJWoTWJVPMZAg7wbKy6SI4fR6jCWyyqYdjaqzNg==";

            var response = await client.GetAsync(route);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return records;
            }

            string json = await response.Content.ReadAsStringAsync();
            records = JsonConvert.DeserializeObject<List<AuditRecord>>(json);

            return records;
        }
    }
}