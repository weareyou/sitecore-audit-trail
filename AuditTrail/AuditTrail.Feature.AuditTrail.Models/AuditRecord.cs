using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditTrail.Feature.AuditTrail.Models
{
    /*
     * AuditRecord needs to be able to represent every possible audited event.
     * Any event-specific data is stored in EventData. Events which do not apply should be kept null.
     * Though not enforced, only one event in EventData should be utilized. Group events by creating collections of AuditRecords,
     * in order to preserve the context of each event.
     */
    public class AuditRecord
    {
        public string SitecoreInstanceName { get; set; }
        public string Event { get; set; } = "Undefined";
        public string ItemId { get; set; }
        public string ItemName { get; set; }

        public string User { get; set; } //TODO: technically redundant but easier to access than the "__Updated by" field. Keep?

        public string TemplateName { get; set; }

        public string EventOrigin { get; set; }

        public DateTime Timestamp { get; set; }

        public EventData EventData { get; set; } = new EventData();
    }
}