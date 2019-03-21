using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AuditTrail.Feature.AuditTrail.Models;
using AuditTrail.Feature.AuditTrail.Network;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace AuditTrail.Feature.AuditTrail.Commands
{
    public class ShowItemHistory : Command
    {
        //TODO: replace this dumpster fire with a maintainable solution
        public override void Execute(CommandContext context)
        {
            //SheerResponse.Alert("This should show the item's history.", false);
            string itemId = context.Parameters["id"];
            /*
            var task = ShowItemHistoryPopup(itemId);
            task.Wait();
            task.*/
            try
            {
                List<AuditRecord> records = Task.Run(async () => { return await FunctionRequests.GetItemHistory(itemId); }).Result;

                string html = File.ReadAllText("/assets/index.html");
                string notfound = File.ReadAllText("/assets/notfound.html");

               
                if (records.Count < 1)
                {
                    SheerResponse.ShowPopup("itemhistory", "center", notfound);
                    return;
                }

                string body = "";

                foreach (AuditRecord record in records)
                {
                    body += "<tr>";
                    body += "<td>" + record.Timestamp.ToShortTimeString() + "</td>";
                    body += "<td>" + record.Event + "</td>";

                    
                    string eventData = "";
                    //savedevent
                    if (record.EventData.Saved != null)
                    {
                        if (record.EventData.Saved.Fields.Count > 0)
                        {
                            eventData += "<p>Fields:<p>";
                            foreach(var change in record.EventData.Saved.Fields)
                            {
                                eventData += "<p style='margin-left:8px;'>" + change.Value.FieldName + "</p>";
                                eventData += "<p style='margin-left:16px;'> New value:" + change.Value.NewValue + "</p>";
                                eventData += "<p style='margin-left:16px;'> Old value:" + change.Value.OldValue + "</p>";
                            }
                        }

                        if (record.EventData.Saved.Properties.Count > 0)
                        {
                            eventData += "<p>Properties:<p>";
                            foreach (var change in record.EventData.Saved.Properties)
                            {
                                eventData += "<p style='margin-left:8px;'>" + change.Value.PropertyName + "</p>";
                                eventData += "<p style='margin-left:16px;'> New value:" + change.Value.NewValue + "</p>";
                                eventData += "<p style='margin-left:16px;'> Old value:" + change.Value.OldValue + "</p>";
                            }
                        }
                    }

                    else if (record.EventData.Copied != null)
                    {

                    }

                    if (eventData == "") eventData = "No internal changes occured.";
                    body += "<td>" + eventData +"</td>";
                    body += "<tr>";

                }

                int tableBodyStart = html.IndexOf("<tbody>") + 7;

                html = html.Insert(tableBodyStart, body);
                SheerResponse.ShowPopup("itemhistory", "center", html);
            }
            catch (Exception e)
            {
                SheerResponse.Alert("Could not retrieve item history: " + e.Message, false);
            }




            













        }

        public override CommandState QueryState(CommandContext context)
        {
            return base.QueryState(context);
        }

       
    }
}


//string page = FunctionRequests.GetItemViewHTML().Result;
//var task = Task.Run(async () => await FunctionRequests.GetItemViewHTML());
//SheerResponse.ShowPopup("itemhistory", "center", task.Result );


/*SheerResponse.ShowModalDialog(new ModalDialogOptions("/sitecore/client/Your Apps/AuditTrail/ItemHistory")
{
    Width = "200",
    Height = "400",
    Message = "queryparam",
    Response = false,
    ForceDialogSize = true
});*/

//int headers = html.IndexOf("<thead>");
//html.Insert(html.IndexOf())
//var menu = new Sitecore.Web.UI.HtmlControls.Frame();
//var p = SheerResponse.ShowPopup("itemhistory", "below-right", html);