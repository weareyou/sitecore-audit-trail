using AuditTrail.Feature.AuditTrail.Helpers;
using AuditTrail.Feature.AuditTrail.Models.Events;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Items;
using Sitecore.Data.SqlServer;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using AuditTrail.Feature.AuditTrail.Models;

namespace AuditTrail.Feature.AuditTrail.DataProviders
{
    /*
     * AuditTrail hooks into DataProvider in order to avoid EventDisabler on the Event layer.
     * This provider is NOT meant to to replace the default Provider, and simply passes all event calls through.
     * 
     * All overrides must return true.
     */
    public class Auditing : SqlServerDataProvider
    {
        public Auditing(string connectionString) : base(connectionString)
        {

        }

        public void PopulateDefaultFields(ItemDefinition item, AuditRecord record)
        {
            record.ItemId = item.ID.ToString();
            record.ItemName = item.Name;
            record.SitecoreInstanceName = Sitecore.Globals.ServerUrl;
            record.User = Sitecore.Context.User.Name;
            record.Timestamp = DateTime.Now;
            record.TemplateName = Database.GetDatabase("master").GetTemplate(item.TemplateID).Name;


            // Event Origin: CE, EE, API or other(code)
            // TODO: this is not quite accurate yet
            string task = "";
            if (Sitecore.Context.Task != null)
                task = Sitecore.Context.Task.Name;
            bool ee = Sitecore.Context.PageMode.IsExperienceEditor;
            if (task == "Sheer" && !ee)
            {
                record.EventOrigin = "Content Editor";
            } 
            else if (ee)
            {
                record.EventOrigin = "Experience Editor";
            }
            else
            {
                record.EventOrigin = "Automated";
            }


        }

        // Events
        // Two CreateItems in the base provider, first is possibly to be deprecated?
        public override bool CreateItem(ID itemId, string itemName, ID templateId, ItemDefinition parent, CallContext context)
        {
            return CreateItem(itemId, itemName, templateId, parent, DateTime.Now, context);
        }
        public override bool CreateItem(ID itemId, string itemName, ID templateId, ItemDefinition parent, DateTime created, CallContext context)
        {
            var item = GetItemDefinition(itemId, context);

            AuditRecord record = new AuditRecord();
            record.Event = "Created";
            PopulateDefaultFields(item, record);

            record.EventData.Created = new Created();

            EventDataHelper.StoreRecord(record);

            return true;
        }

        public override bool SaveItem(ItemDefinition item, ItemChanges changes, CallContext context)
        {
            AuditRecord record = new AuditRecord();
            record.Event = "Saved";
            PopulateDefaultFields(item, record);

            record.EventData.Saved = new Saved();

            record.EventData.Saved.Fields = EventDataHelper.StoreFieldChanges(changes);
            record.EventData.Saved.Properties = EventDataHelper.StorePropertyChanges(changes);

            // careful; if default fields are filtered out, this will skip an event from being saved alltogether
            if (record.EventData.Saved.Fields.Count != 0 || record.EventData.Saved.Properties.Count != 0)
                EventDataHelper.StoreRecord(record);

            return true;
        }

        public override bool MoveItem(ItemDefinition item, ItemDefinition destination, CallContext context)
        {
            AuditRecord record = new AuditRecord();
            record.Event = "Moved";
            PopulateDefaultFields(item, record);

            record.EventData.Moved = new Moved();

            record.EventData.Moved.DestinationPath = Database.GetDatabase("master").Items.GetItem(destination.ID).Paths.ContentPath;

            EventDataHelper.StoreRecord(record);

            return true;
        }

        public override bool CopyItem(ItemDefinition item, ItemDefinition destination, string copyName, ID copyID, CallContext context)
        {
            AuditRecord record = new AuditRecord();
            record.Event = "Copied";
            PopulateDefaultFields(item, record);

            record.EventData.Copied = new Copied();

            record.EventData.Copied.ItemPath = Database.GetDatabase("master").Items.GetItem(item.ID).Paths.ContentPath;

            record.EventData.Copied.ItemIdCopy = destination.ID.ToString();
            record.EventData.Copied.ItemNameCopy = destination.Name;
            record.EventData.Copied.ItemPathCopy = Database.GetDatabase("master").Items.GetItem(destination.ID).Paths.ContentPath;

            EventDataHelper.StoreRecord(record);

            return true;
        }

        public override bool DeleteItem(ItemDefinition item, CallContext context)
        {
            AuditRecord record = new AuditRecord();
            record.Event = "Deleted";
            PopulateDefaultFields(item, record);

            record.EventData.Deleted = new Deleted();

            EventDataHelper.StoreRecord(record);

            return true;
        }

        // Versioning

        public override int AddVersion(ItemDefinition item, VersionUri baseVersion, CallContext context)
        {
            var baseVersionResult = baseVersion.Version.Number + 1;

            AuditRecord record = new AuditRecord();
            record.Event = "VersionAdded";
            PopulateDefaultFields(item, record);

            record.EventData.VersionAdded = new VersionAdded();

            record.EventData.VersionAdded.Version = baseVersionResult;

            EventDataHelper.StoreRecord(record);


            return baseVersion.Version.Number;
        }

        public override bool RemoveVersion(ItemDefinition item, VersionUri version, CallContext context)
        {
            AuditRecord record = new AuditRecord();
            record.Event = "VersionRemoved";
            PopulateDefaultFields(item, record);

            record.EventData.VersionRemoved = new VersionRemoved();

            record.EventData.VersionRemoved.Version = version.Version.Number;

            EventDataHelper.StoreRecord(record);

            return true;
        }

        public override bool RemoveVersions(ItemDefinition itemDefinition, Language language, bool removeSharedData, CallContext context)
        {

            return true;
        }







        // Default read, pass through to default DataProvider
        
        public override IDList GetChildIDs(ItemDefinition item, CallContext context)
        {
            return new IDList();
        }

        public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
        {

            return base.GetItemDefinition(itemId, context);
        }

        public override FieldList GetItemFields(ItemDefinition itemDefinition, VersionUri versionUri, CallContext context)
        {

            return base.GetItemFields(itemDefinition, versionUri, context);
        }

        public override VersionUriList GetItemVersions(ItemDefinition itemDefinition, CallContext context)
        {

            return base.GetItemVersions(itemDefinition, context);
        }

        public override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
        {

            return base.GetParentID(itemDefinition, context);
        }
        


    }
}