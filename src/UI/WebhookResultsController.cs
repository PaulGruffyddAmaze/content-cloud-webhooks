using DeaneBarker.Optimizely.Webhooks.Blocks;
using DeaneBarker.Optimizely.Webhooks.Stores;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Security;
using EPiServer.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IWebhookStore _store;

        public WebhookResultsController(ContentLoader contentLoader, IWebhookStore store)
        {
            _contentLoader = contentLoader;
            _store = store;
        }

        [HttpGet]
        [Route("opti/webhooks/results", Name = "webhookResults")]
        public ActionResult Index(string id = "0")
        {
            var content = new ContentReference(id);
            var webhookContent = _contentLoader.Get<WebhookFactoryBlock>(content);
            
            //Don't let a user access this page if they don't have access to edit the webhook
            if (!(webhookContent as IContentSecurable).GetContentSecurityDescriptor().HasAccess(User, AccessLevel.Edit))
            {
                return Unauthorized();
            }

            var results = _store.GetResultsByFactoryId(webhookContent.FactoryId);
            foreach (var result in results)
            {
                result.contentName = _contentLoader.Get<IContent>(result.contentLink).Name;
            }
            return View(new WebhookResultsViewModel { CurrentBlock =  webhookContent, Results = results });
        }
    }
}
