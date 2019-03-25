using AuditTrail.Feature.AuditTrail.Models;
using AuditTrail.Feature.AuditTrail.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditTrail.Feature.AuditTrail.Helpers
{
    public static class EventDataHelper
    {
        // make these config variables?
        public static bool IgnoreDefaultItemFields { get; set; } = true;
        public static bool IgnoreScheduledEvents { get; set; } = true;


        public static Dictionary<string, FieldChange> StoreFieldChanges(Sitecore.Data.Items.ItemChanges itemChanges)
        {
            var fields = new Dictionary<string, FieldChange>();

            foreach (Sitecore.Data.Items.FieldChange fieldChange in itemChanges.FieldChanges)
            {
                FieldChange fieldChangeRecord = new FieldChange();
                fieldChangeRecord.FieldId = fieldChange.FieldID.ToString();
                fieldChangeRecord.NewValue = fieldChange.Value;
                fieldChangeRecord.OldValue = fieldChange.OriginalValue;


                // Empty fields lack a "Definition" and require a placeholder name.
                if (fieldChange.Definition != null)
                    fieldChangeRecord.FieldName = fieldChange.Definition.Name;
                else
                {
                    fieldChangeRecord.FieldName = "Undefined Field";
                }


                if (!(IsDefaultItemField(fieldChangeRecord.FieldName) && IgnoreDefaultItemFields))
                    fields.Add(fieldChangeRecord.FieldName, fieldChangeRecord);

            }

            return fields;
        }

        public static Dictionary<string, PropertyChange> StorePropertyChanges(Sitecore.Data.Items.ItemChanges itemChanges)
        {
            var properties = new Dictionary<string, PropertyChange>();

            foreach (var propertyChange in itemChanges.Properties)
            {
                PropertyChange propertyChangeRecord = new PropertyChange();

                propertyChangeRecord.NewValue = propertyChange.Value.Value.ToString();
                propertyChangeRecord.OldValue = propertyChange.Value.OriginalValue.ToString();
                propertyChangeRecord.PropertyName = propertyChange.Value.Name;

                properties.Add(propertyChangeRecord.PropertyName, propertyChangeRecord);

            }

            return properties;
        }

        public static void StoreRecord(AuditRecord record)
        {
            if (IgnoreScheduledEvents && IsScheduledEvent(record))
                return;

            if (AuditAggregator.Instance.Aggregating)
            {
                AuditAggregator.Instance.AddAuditRecord(record);
            }
            else
            {
                FunctionRequests.SendEventData(record);
            }
        }

        public static bool IsDefaultItemField(string fieldName)
        {
            if (fieldName.StartsWith("__"))
                return true;
            return false;
        }

        public static bool IsScheduledEvent(AuditRecord record)
        {
            if (record.TemplateName == "Schedule")
                return true;
            return false;
        }
    }
}