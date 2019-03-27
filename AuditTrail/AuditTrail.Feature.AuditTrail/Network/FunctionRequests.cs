using AuditTrail.Feature.AuditTrail.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuditTrail.Feature.AuditTrail.Network
{
    public static class FunctionRequests
    {
        public static void SendEventData(AuditRecord record)
        {
            var recordJson = JsonConvert.SerializeObject(record);
            PostJson(recordJson);
        }

        public static void SendEventData(UserInteractionRecord record)
        {
            var recordJson = JsonConvert.SerializeObject(record);
            PostJson(recordJson);
        }

        private static void PostJson(string json)
        {
            var client = new HttpClient();

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.PostAsync(Properties.Resources.STORE_EVENT_DATA_FUNCTION_URL, content);
        }
        
        public static async Task<List<AuditRecord>> GetItemHistory(string itemId)
        {
            var client = new HttpClient();

            var records = new List<AuditRecord>();
            var route = "http://audit-trail.azurewebsites.net/api/item/" + itemId + "?code=wV12VHq7F0ACsu0FJWoTWJVPMZAg7wbKy6SI4fR6jCWyyqYdjaqzNg==";

            var response = await client.GetAsync(route);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return records;
            }

            var json = await response.Content.ReadAsStringAsync();
            records = JsonConvert.DeserializeObject<List<AuditRecord>>(json);

            return records;
        }
    }
}