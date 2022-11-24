using DeaneBarker.Optimizely.Webhooks.Blocks;
using EPiServer.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.UI
{
    [UIDescriptorRegistration]
    public class WebhookUIDescriptor : UIDescriptor<WebhookFactoryBlock>
    {
        public WebhookUIDescriptor()
        {
            IconClass = "epi-iconReferences";
            DefaultView = CmsViewNames.AllPropertiesView;
            AddDisabledView(CmsViewNames.PreviewView);
            AddDisabledView(CmsViewNames.OnPageEditView);
        }
    }
}
