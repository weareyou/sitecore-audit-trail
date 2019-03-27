using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitecoreLogicConnector.Feature.SLC.Events
{
    public abstract class BaseEvent
    {
        public abstract void CallWebhook(object sender, EventArgs args);
    }
}