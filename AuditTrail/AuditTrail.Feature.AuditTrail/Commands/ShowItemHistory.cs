using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AuditTrail.Feature.AuditTrail.Models;
using AuditTrail.Feature.AuditTrail.Network;
using Sitecore.Configuration;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace AuditTrail.Feature.AuditTrail.Commands
{
    public class ShowItemHistory : Command
    {
        //TODO: replace this dumpster fire with a maintainable solution
        public override void Execute(CommandContext context)
        {
            string itemId = "";

            Assert.ArgumentNotNull((object)context, nameof(context));
            if (context.Items.Length  > 0)
            {
                itemId = context.Items[0].ID.ToString();
            }
            else {
                itemId = context.Parameters["id"];
            }

                
            try
            {
                List<AuditRecord> records = Task.Run(async () => { return await FunctionRequests.GetItemHistory(itemId); }).Result;

                string websitePath = Sitecore.IO.FileUtil.MapPath("/");
                string path = websitePath + "Assets/index.html";
                string html = File.ReadAllText(websitePath + "Assets/index.html");
                string notfound = File.ReadAllText(websitePath + "Assets/notfound.html");

               
                if (records.Count < 1)
                {
                    SheerResponse.ShowPopup("itemhistory", "center", notfound);
                    return;
                }

                string body = "";

                foreach (AuditRecord record in records)
                {
                    body += "<tr>";
                    body += "<td>" + record.Timestamp.ToShortTimeString() + " " + record.Timestamp.ToShortDateString() + "</td>";
                    body += "<td>" + record.Event + "</td>";

                    
                    string eventData = "";
                    // Saved
                    if (record.EventData.Saved != null)
                    {
                        if (record.EventData.Saved.Fields.Count > 0)
                        {
                            eventData += "<p style='font-weight:bold'>Fields:</p>";
                            foreach(var change in record.EventData.Saved.Fields)
                            {
                                eventData += "<p style='margin-left:15px; font-style:italic;'>" + change.Value.FieldName + " " + change.Value.FieldId + "</p>";
                                eventData += "<p style='margin-left:30px;'>New value:" + change.Value.NewValue + "</p>";
                                eventData += "<p style='margin-left:30px;'>Old value:" + change.Value.OldValue + "</p>";
                                eventData += "<br>";
                            }
                        }

                        if (record.EventData.Saved.Properties.Count > 0)
                        {
                            eventData += "<p style='font-weight:bold'>Properties:</p>";
                            foreach (var change in record.EventData.Saved.Properties)
                            { 
                                eventData += "<p style='margin-left:15px; font-style:italic;'>" + change.Value.PropertyName + "</p>";
                                eventData += "<p style='margin-left:30px;'>New value:  " + change.Value.NewValue + "</p>";
                                eventData += "<p style='margin-left:30px;'>Old value:  " + change.Value.OldValue + "</p>";
                                eventData += "<br>";
                            }
                        }
                    }

                    // Copied
                    else if (record.EventData.Copied != null)
                    {
                        eventData += "<p>Copy name: " + record.EventData.Copied.ItemNameCopy + "</p>";
                        eventData += "<p>Copy path: " + record.EventData.Copied.ItemPathCopy + "</p>";
                        eventData += "<p>Copy guid: " + record.EventData.Copied.ItemIdCopy + "</p>";
                    }

                    // Moved
                    else if (record.EventData.Moved != null)
                    {
                        eventData += "<p>Moved to: " + record.EventData.Moved.DestinationPath + "</p>";
                    }

                    // VersionAdded
                    else if (record.EventData.VersionAdded != null)
                    {
                        eventData += "<p>Added Version: " + record.EventData.VersionAdded.Version + "</p>";
                    }

                    // VersionRemoved
                    else if (record.EventData.VersionRemoved != null)
                    {
                        eventData += "<p>Removed Version: " + record.EventData.VersionRemoved.Version + "</p>";
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