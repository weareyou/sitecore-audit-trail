using System;
using System.Collections.Generic;
using System.Text;

namespace AuditTrail.Feature.AuditTrail.Models.Events
{
    public class Copied
    {
        public string ItemPath { get; set; }

        public string ItemIdCopy { get; set; }
        public string ItemNameCopy { get; set; }
        public string ItemPathCopy { get; set; }
    }
}
