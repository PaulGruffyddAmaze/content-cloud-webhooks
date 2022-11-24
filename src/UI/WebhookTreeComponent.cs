using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.UI
{
    [Component]
    public class WebhookTreeComponent : ComponentDefinitionBase
    {
        public WebhookTreeComponent() : base("epi-cms/component/MainNavigationComponent")
        {
            base.Title = "Webhooks";
            Categories = new[] { "content" };
            PlugInAreas = new[] { PlugInArea.AssetsDefaultGroup };
            SortOrder = 1000;
            base.Settings.Add(new Setting("repositoryKey", "webhooks"));
            base.Settings.Add(new Setting("noDataMessages", new { single = "No webhooks have been configured", multiple = "No webhooks have been configured" }));
        }
    }
}
