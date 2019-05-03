using System;
using System.IO;
using System.Threading.Tasks;
using AuditTrail.Feature.AuditTrail.Network;
using AuditTrail.Feature.AuditTrail.View;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace AuditTrail.Feature.AuditTrail.Commands
{
    public class ShowItemHistory : Command
    {
 
        public override void Execute(CommandContext context)
        {
            string itemId;

            Assert.ArgumentNotNull(context, nameof(context));
            if (context.Items.Length > 0)
            {
                itemId = context.Items[0].ID.ToString();
            }
            else {
                itemId = context.Parameters["id"];
            }
            
            try
            {
                var records = Task.Run(async () => await FunctionRequests.GetItemHistory(itemId)).Result;
                
                SheerResponse.ShowPopup("itemhistory", "center", ItemAuditHtmlBuilder.SmallAuditView(records));
            }
            catch (Exception e)
            {
                SheerResponse.Alert("Could not retrieve item history: " + e.Message, false);
            }
        }

        // ReSharper disable once RedundantOverriddenMember
        public override CommandState QueryState(CommandContext context)
        {
            return base.QueryState(context);
        }
    }
}