using Sitecore.Pipelines.ItemProvider.CreateItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditTrail.Feature.AuditTrail.Processors.ItemProvider
{
    public class CreateItem : CreateItemProcessor
    {
        public override void Process(CreateItemArgs args)
        {
            args.FallbackProvider.CreateItem(args.ItemName, args.Destination, args.TemplateId, args.NewId, args.SecurityCheck);
            args.Handled = true;
        }
    }
}