using AuditTrail.Feature.AuditTrail.Models.Events;

namespace AuditTrail.Feature.AuditTrail.Models
{
    public class EventData
    {
        public Copied Copied { get; set; } = null;

        public Created Created { get; set; } = null;

        public Deleted Deleted { get; set; } = null;

        public Moved Moved { get; set; } = null;

        public Saved Saved { get; set; } = null;

        public VersionAdded VersionAdded { get; set; } = null;

        public VersionRemoved VersionRemoved { get; set; } = null;
    }
}
