using DeaneBarker.Optimizely.Webhooks.Blocks;
using EPiServer.Core;
using EPiServer.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.UI
{
    public class WebhookResultsController : Controller
    {
        private readonly ContentLoader _contentLoader;

        public WebhookResultsController(ContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        [HttpGet]
        [Route("opti/webhooks/results", Name = "bulkUpdate")]
        public ActionResult Index(string id = "0")
        {
            var content = new ContentReference(id);
            var webhookContent = _contentLoader.Get<WebhookFactoryBlock>(content);
            //return Content($"<h1>Results for {webhookContent.Name} go here</h1>", "text/html");
            return View();
        }
    }
}
