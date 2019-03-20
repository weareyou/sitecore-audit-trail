using Sitecore.Pipelines.ItemProvider.SaveItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditTrail.Feature.AuditTrail.Processors.ItemProvider
{
    public class SaveItem : SaveItemProcessor
    {
        public override void Process(SaveItemArgs args)
        {
            args.FallbackProvider.SaveItem(args.Item);
            args.Handled = true;
        }
    }
}