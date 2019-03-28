using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace SitecoreLogicConnector.Feature.SLC.AzureFunctions.Webhook
{
    public class WebhookSubscription : TableEntity
    {
        private string id;

        public string Id
        {
            get => id;
            set
            {
                id = RowKey = value;
            }
        }

        public string CallbackUrl { get; set; }

        public WebhookSubscription()
        {
            PartitionKey = "";
        }
    }
}
