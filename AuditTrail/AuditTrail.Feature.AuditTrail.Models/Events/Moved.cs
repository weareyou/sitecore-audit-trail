﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AuditTrail.Feature.AuditTrail.Models.Events
{
    public class Moved
    {
        public string DestinationPath { get; set; }
        // TODO: original path
    }
}
