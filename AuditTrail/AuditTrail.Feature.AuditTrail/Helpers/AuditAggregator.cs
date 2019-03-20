using AuditTrail.Feature.AuditTrail.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuditTrail.Feature.AuditTrail.Helpers
{
    // TODO: There should always be a better solution than a singleton.
    public sealed class AuditAggregator
    {
        private static AuditAggregator instance = null;

        // public Dictionary<string, UserInteractionRecord> records; // in case of concurrent users / events?
        private UserInteractionRecord uiRecord = null;
        public bool Aggregating { get; private set; } = false;

        private AuditAggregator() {
            // records = new Dictionary<string, UserInteractionRecord>(); 
        }


        public void StartAggregating(string uiEventName)
        {
            uiRecord = new UserInteractionRecord(uiEventName);
            uiRecord.Timestamp = DateTime.Now;
            Aggregating = true;
        }

        public void StopAggregating(string uiEventName)
        {
            Aggregating = false;
        }

        public void AddAuditRecord(AuditRecord record)
        {
            uiRecord.Records.Add(uiRecord.Records.Count.ToString(), record);
        }

        public UserInteractionRecord RetrieveUiRecord()
        {
                if (uiRecord != null)
                {
                    UserInteractionRecord record = uiRecord;
                    uiRecord = null;
                    return record;
                }

            return null;
        }

        public static AuditAggregator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AuditAggregator();   
                }
                return instance;
            }
        }


    }
}