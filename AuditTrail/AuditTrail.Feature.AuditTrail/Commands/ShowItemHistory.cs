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

/*
 if (records.Count < 1)
                {
                    SheerResponse.ShowPopup("itemhistory", "center", notfound);
                    return;
                }

                var body = string.Empty;

                foreach (var record in records)
                {
                    body += "<tr>";
                    body += "<td>" + record.Timestamp.ToShortTimeString() + " " + record.Timestamp.ToShortDateString() + "</td>";
                    body += "<td>" + record.Event + "</td>";
                    
                    var eventData = string.Empty;
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

                var tableBodyStart = html.IndexOf("<tbody>", StringComparison.Ordinal) + 7;

                html = html.Insert(tableBodyStart, body);
*/