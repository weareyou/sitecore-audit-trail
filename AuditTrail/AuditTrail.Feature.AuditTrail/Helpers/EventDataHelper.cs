using AuditTrail.Feature.AuditTrail.Models;
using AuditTrail.Feature.AuditTrail.Network;
using System.Collections.Generic;

namespace AuditTrail.Feature.AuditTrail.Helpers
{
    public static class EventDataHelper
    {
        public static bool IgnoreDefaultItemFields { get; set; } = true;

        public static bool IgnoreScheduledEvents { get; set; } = true;

        public static Dictionary<string, FieldChange> StoreFieldChanges(Sitecore.Data.Items.ItemChanges itemChanges)
        {
            var fields = new Dictionary<string, FieldChange>();

            foreach (Sitecore.Data.Items.FieldChange fieldChange in itemChanges.FieldChanges)
            {
                var fieldChangeRecord = new FieldChange
                {
                    FieldId = fieldChange.FieldID.ToString(),
                    NewValue = fieldChange.Value,
                    OldValue = fieldChange.OriginalValue
                };

                // empty fields lack a "Definition" and require a placeholder name
                if (fieldChange.Definition != null)
                {
                    fieldChangeRecord.FieldName = fieldChange.Definition.Name;
                }
                else
                {
                    fieldChangeRecord.FieldName = $"Undefined field ({fieldChangeRecord.FieldId})";
                }

                if (!(IsDefaultItemField(fieldChangeRecord.FieldName) && IgnoreDefaultItemFields))
                {
                    fields.Add(fieldChangeRecord.FieldName, fieldChangeRecord);
                }
            }

            return fields;
        }

        public static Dictionary<string, PropertyChange> StorePropertyChanges(Sitecore.Data.Items.ItemChanges itemChanges)
        {
            var properties = new Dictionary<string, PropertyChange>();

            foreach (var propertyChange in itemChanges.Properties)
            {
                var propertyChangeRecord = new PropertyChange
                {
                    NewValue = propertyChange.Value.Value.ToString(),
                    OldValue = propertyChange.Value.OriginalValue.ToString(),
                    PropertyName = propertyChange.Value.Name
                };
                
                properties.Add(propertyChangeRecord.PropertyName, propertyChangeRecord);

            }

            return properties;
        }

        public static void StoreRecord(AuditRecord record)
        {
            if (IgnoreScheduledEvents && IsScheduledEvent(record))
                return;

            FunctionRequests.SendEventData(record);
        }

        public static bool IsDefaultItemField(string fieldName)
        {
            return fieldName.StartsWith("__");
        }

        public static bool IsScheduledEvent(AuditRecord record)
        {
            return record.TemplateName.Equals("Schedule");
        }
    }
}