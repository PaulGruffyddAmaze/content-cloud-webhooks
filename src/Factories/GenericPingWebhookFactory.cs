using DeaneBarker.Optimizely.Webhooks;
using DeaneBarker.Optimizely.Webhooks.Blocks;
using DeaneBarker.Optimizely.Webhooks.Factories;
using DeaneBarker.Optimizely.Webhooks.Helpers;
using DeaneBarker.Optimizely.Webhooks.Serializers;
using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Ayogo3.Webhooks.Factories
{
    public class GenericPingWebhookFactory : IWebhookFactoryProfile
    {
        public IEnumerable<Webhook> Process(string action, IWebhookFactory webhookProfile, IContent content)
        {
            if(action != "View")
            {
                return null;
            }
            return new[] { new Webhook(new Uri("https://cnn.com"), action, webhookProfile, new UrlPingSerializer()) };
        }
    }

    public class UrlPingSerializer : IWebhookSerializer
    {
        public HttpRequestMessage Serialize(Webhook webhook)
        {
            return new WebRequestBuilder().AsGet().ToUrl("https://cnn.com").WithQuerystringArg("action", webhook.Action.ToLower()).Build();
        }
    }
}