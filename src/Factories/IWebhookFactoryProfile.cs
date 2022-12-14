using DeaneBarker.Optimizely.Webhooks.Blocks;
using DeaneBarker.Optimizely.Webhooks.Factories;
using EPiServer.Core;
using System;
using System.Collections.Generic;

namespace DeaneBarker.Optimizely.Webhooks
{
    public interface IWebhookFactoryProfile
    {
        IEnumerable<Webhook> Process(string action, IWebhookFactory webhookProfile, IContent content);
    }
}