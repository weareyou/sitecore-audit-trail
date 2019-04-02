using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using AuditTrail.Feature.AuditTrail.Models;
using Sitecore.Shell.Applications.Install.Commands;

namespace AuditTrail.Feature.AuditTrail.View
{
    public static class ItemAuditHtmlBuilder
    {
        public static string SmallAuditView(List<AuditRecord> records)
        {
            var websitePath = Sitecore.IO.FileUtil.MapPath("/");
            var index = File.ReadAllText(websitePath + "Assets/index.html");
            var notfound = File.ReadAllText(websitePath + "Assets/notfound.html");


            if (records.Count < 1)
            {
                return notfound;
            }
            else
            {
                var tableBodyStart = index.IndexOf("<tbody>", StringComparison.Ordinal) + 7;

                index = index.Insert(tableBodyStart, BuildTableRows(records));

                return index;
            }
        }

        private static string BuildTableRows(List<AuditRecord> records)
        {
            StringBuilder html = new StringBuilder();

            foreach (var record in records)
            {
                html.Append("<tr>");
                html.AppendFormat("<td>{0} {1}</td>", record.Timestamp.ToShortTimeString(), record.Timestamp.ToShortDateString());
                html.AppendFormat("<td>{0}</td>", record.Event);
                html.Append("<td>");

                // Saved
                if (record.EventData.Saved != null)
                {
                    if (record.EventData.Saved.Fields.Count > 0)
                    {
                        html.Append("<p style='font-weight:bold'>Fields:</p>");
                        foreach (var change in record.EventData.Saved.Fields)
                        {
                            html.AppendFormat("<p style='margin-left:15px; font-style:italic;'>{0} {1}</p>", change.Value.FieldName, change.Value.FieldId);
                            html.AppendFormat("<p style='margin-left:30px;'>New value: {0}</p>", change.Value.NewValue);
                            html.AppendFormat("<p style='margin-left:30px;'>Old value: {0}</p>", change.Value.OldValue);
                            html.Append("<br>");
                        }
                    }

                    if (record.EventData.Saved.Properties.Count > 0)
                    {
                        html.Append("<p style='font-weight:bold'>Properties:</p>");
                        foreach (var change in record.EventData.Saved.Properties)
                        {
                            html.AppendFormat("<p style='margin-left:15px; font-style:italic;'>{0}</p>", change.Value.PropertyName);
                            html.AppendFormat("<p style='margin-left:30px;'>New value: {0}</p>", change.Value.NewValue);
                            html.AppendFormat("<p style='margin-left:30px;'>Old value: {0}</p>", change.Value.OldValue);
                            html.Append("<br>");
                        }
                    }
                }

                // Copied
                else if (record.EventData.Copied != null)
                {
                    html.AppendFormat("<p>Copy name: {0}</p>", record.EventData.Copied.ItemNameCopy);
                    html.AppendFormat("<p>Copy path: {0}</p>", record.EventData.Copied.ItemPathCopy);
                    html.AppendFormat("<p>Copy guid: {0}</p>", record.EventData.Copied.ItemIdCopy);
                }

                // Moved
                else if (record.EventData.Moved != null)
                {
                    html.AppendFormat("<p>Moved to: {0}</p>", record.EventData.Moved.DestinationPath);
                }

                // VersionAdded
                else if (record.EventData.VersionAdded != null)
                {
                    html.AppendFormat("<p>Added Version: {0}</p>", record.EventData.VersionAdded.Version);
                }

                // VersionRemoved
                else if (record.EventData.VersionRemoved != null)
                {
                    html.AppendFormat("<p>Removed Version: {0}</p>", record.EventData.VersionRemoved.Version);
                }
                else
                {
                    html.Append("No internal changes occured.");
                }

                html.Append("</td></tr>");

            }

            return html.ToString();
        }
    }
}