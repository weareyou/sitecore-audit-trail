using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditTrail.Feature.AuditTrail.Models
{
    public class PropertyChange
    {
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string PropertyName { get; set; }
    }
}