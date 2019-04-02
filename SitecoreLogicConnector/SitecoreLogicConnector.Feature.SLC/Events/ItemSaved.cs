using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace SitecoreLogicConnector.Feature.SLC.Events
{
    public class ItemSaved
    {
        public static bool IgnoreScheduledEvents { get; set; } = true;

        public void CallWebHook(object sender, EventArgs args)
        {
            var itemChanges = Sitecore.Events.Event.ExtractParameter(args, 1) as Sitecore.Data.Items.ItemChanges;

            if (itemChanges == null) return;
            if (IgnoreScheduledEvents && itemChanges.Item.TemplateName.Equals("Schedule")) return;

            var itemId = itemChanges.Item.ID;
            var itemName = itemChanges.Item.Name;

            var fieldChangePayloads = new Dictionary<string, FieldChangePayload>();

            foreach (Sitecore.Data.Items.FieldChange fieldChange in itemChanges.FieldChanges)
            {
                var fieldChangePayload = new FieldChangePayload
                {
                    FieldId = fieldChange.FieldID.ToString(),
                    Value = fieldChange.Value,
                    OriginalValue = fieldChange.OriginalValue,
                    FieldName = fieldChange.Definition.Name
                };

                fieldChangePayloads.Add(fieldChangePayload.FieldName, fieldChangePayload);

            }

            var itemJson = new ItemSavedJson
            {
                ItemId = itemId.ToString(), ItemName = itemName, FieldChanges = fieldChangePayloads
            };


            var itemJsonString = JsonConvert.SerializeObject(itemJson);

            var content = new StringContent(itemJsonString, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            client.PostAsync(Properties.Resources.AzureFunctionsConnectorURL + "/ItemSaved?code=" + Properties.Resources.AzureFunctionsConnectorAuthCode, content);
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