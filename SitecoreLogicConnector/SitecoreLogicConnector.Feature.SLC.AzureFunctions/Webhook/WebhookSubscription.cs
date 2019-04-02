using Microsoft.WindowsAzure.Storage.Table;

namespace SitecoreLogicConnector.Feature.SLC.AzureFunctions.WebHook
{
    public class WebHookSubscription : TableEntity
    {
        private string _id;

        public string Id
        {
            get => _id;
            set => _id = RowKey = value;
        }

        public string CallbackUrl { get; set; }

        public WebHookSubscription()
        {
            PartitionKey = "";
        }
    }
}
