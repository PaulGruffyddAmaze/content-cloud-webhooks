using DeaneBarker.Optimizely.Webhooks.Blocks;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.UI
{
    [ServiceConfiguration(typeof(EPiServer.Shell.ViewConfiguration))]
    public class WebhookResultsViewConfiguration : ViewConfiguration<WebhookFactoryBlock>
    {
        public WebhookResultsViewConfiguration()
        {
            Key = "WebhookResults";
            Name = "Webhook attempt results";
            Description = "Information on the outcomes of webhook attempts";
            ControllerType = "epi-cms/widget/IFrameController";
            ViewType = "/opti/webhooks/results";
            IconClass = "epi-iconGraph";
        }
    }
}
