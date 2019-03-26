using System.Collections.Generic;

namespace AuditTrail.Feature.AuditTrail.Models.Events
{
    public class Saved
    {
        public Dictionary<string, FieldChange> Fields { get; set; }

        public Dictionary<string, PropertyChange> Properties { get; set; }
    }
}
