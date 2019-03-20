using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditTrail.Feature.AuditTrail.Models
{
    public class FieldChange
    {
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string FieldId { get; set; }
        public string FieldName { get; set; }
    }
}