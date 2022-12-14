using DeaneBarker.Optimizely.Webhooks;
using DeaneBarker.Optimizely.Webhooks.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.UI
{
    public class WebhookResultsViewModel
    {
        public WebhookFactoryBlock CurrentBlock { get; set; }
        public IEnumerable<StorableWebhook> Results { get; set; }
    }
}
