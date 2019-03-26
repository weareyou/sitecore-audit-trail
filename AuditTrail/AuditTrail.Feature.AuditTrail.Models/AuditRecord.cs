using System;

namespace AuditTrail.Feature.AuditTrail.Models
{
    /// <summary>
    /// AuditRecord needs to be able to represent every possible audited event.
    /// Any event-specific data is stored in EventData. Events which do not apply should be kept null.
    /// Though not enforced, only one event in EventData should be utilized.
    /// Group events by creating collections of AuditRecords in order to preserve the context of each event.
    /// </summary>
    public class AuditRecord
    {
        public string SitecoreInstanceName { get; set; }

        /// <summary>
        /// Populated by "Undefined" as a default value to be able to recognize empty events created by an empty (Template) save.
        /// </summary>
        public string Event { get; set; } = "Undefined";

        public string ItemId { get; set; }

        public string ItemName { get; set; }

        /// <summary>
        /// Technically redundant (to the "__Updated by" field) but implemented because of the fact that we ignore Sitecore fields prefixed with __.
        /// </summary>
        public string User { get; set; }

        public string TemplateName { get; set; }

        public string EventOrigin { get; set; }

        public DateTime Timestamp { get; set; }

        public EventData EventData { get; set; } = new EventData();
    }
}