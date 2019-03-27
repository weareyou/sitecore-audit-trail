
namespace AuditTrail.Feature.AuditTrail.Models.Events
{
    public class Moved
    {
        public string DestinationPath { get; set; }
        
        // TODO: create original path class member and populate on event trigger
    }
}
