using AuditTrail.Feature.AuditTrail.Helpers;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AuditTrail.Feature.AuditTrail.Models;
using AuditTrail.Feature.AuditTrail.Network;
using Sitecore.Data.Events;

namespace AuditTrail.Feature.AuditTrail.Processors
{
    public class ProcessAuditor
    {
        private string eventName;

        public bool Auditing { get; private set; }
        public ProcessAuditor(string eventName)
        {
            this.eventName = eventName;
            Auditing = false;
        }
        public void ProcessAuditStart(ClientPipelineArgs args)
        {
            AuditAggregator.Instance.StartAggregating(eventName);

            Auditing = true;
        }

        public void ProcessAuditEnd(ClientPipelineArgs args)
        {
            AuditAggregator.Instance.StopAggregating(eventName);
            FunctionRequests.SendEventData(AuditAggregator.Instance.RetrieveUiRecord());

            Auditing = false;
        }
    }
}