using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditTrail.Feature.AuditTrail.Models
{
    public class UserInteractionRecord
    {
        public UserInteractionRecord(string uiEventName)
        {
            EventName = uiEventName;
            Records = new Dictionary<string, AuditRecord>();
        }

        public string EventName { get; set; }

        public DateTime Timestamp { get; set; }

        public Dictionary<string, AuditRecord> Records { get; set; }
    }
}