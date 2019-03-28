using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Sitecore.Diagnostics;

namespace SitecoreLogicConnector.Feature.SLC.Events
{
    public class ItemSaved
    {
        public void CallWebhook(object sender, EventArgs args)
        {
            HttpClient client = new HttpClient();

            var item = Sitecore.Events.Event.ExtractParameter(args, 0) as Sitecore.Data.Items.Item;
            var itemChanges = Sitecore.Events.Event.ExtractParameter(args, 1) as Sitecore.Data.Items.ItemChanges;

            var itemId = itemChanges.Item.ID;
            var itemName = itemChanges.Item.Name;


            var fieldChangePayloads = new Dictionary<string, FieldChangePayload>();

            foreach (Sitecore.Data.Items.FieldChange fieldChange in itemChanges.FieldChanges)
            {
                FieldChangePayload fieldChangePayload = new FieldChangePayload();
                fieldChangePayload.FieldId = fieldChange.FieldID.ToString();
                fieldChangePayload.Value = fieldChange.Value;
                fieldChangePayload.OriginalValue = fieldChange.OriginalValue;
                fieldChangePayload.FieldName = fieldChange.Definition.Name;

                fieldChangePayloads.Add(fieldChangePayload.FieldName, fieldChangePayload);

            }

            ItemSavedJson itemJson = new ItemSavedJson();
            itemJson.ItemId = itemId.ToString();
            itemJson.ItemName = itemName;
            itemJson.FieldChanges = fieldChangePayloads;


            string itemJsonString = JsonConvert.SerializeObject(itemJson);

            var content = new StringContent(itemJsonString, Encoding.UTF8, "application/json");

            var response = client.PostAsync(Properties.Resources.AzureFunctionsConnectorURL + "/ItemSaved?code=" + Properties.Resources.AzureFunctionsConnectorAuthCode, content);
        }

        private class FieldChangePayload
        {
            public string Value;
            public string OriginalValue;
            public string FieldId;
            public string FieldName;
        }

        private class ItemSavedJson
        {
            public string ItemId;
            public string ItemName;
            public Dictionary<string, FieldChangePayload> FieldChanges;
        }
    }
}