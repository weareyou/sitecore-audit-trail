using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace AuditTrail.Feature.AuditTrail.Commands
{
    public class ShowItemHistory : Command
    {
        public override void Execute(CommandContext context)
        {
            //SheerResponse.Alert("This should show the item's history.", false);

            string webpath = "https://sitecoreaudittrail.blob.core.windows.net/$web";



            string html = File.ReadAllText("/assets/popup.html");
            SheerResponse.ShowPopup("itemhistory", "center", html);
            /*SheerResponse.ShowModalDialog(new ModalDialogOptions("/sitecore/client/Your Apps/AuditTrail/ItemHistory")
            {
                Width = "200",
                Height = "400",
                Message = "queryparam",
                Response = false,
                ForceDialogSize = true
            });*/

        }

        public override CommandState QueryState(CommandContext context)
        {
            return base.QueryState(context);
        }

       
    }
}