﻿using DeaneBarker.Optimizely.Webhooks.Blocks;
using EPiServer.Core;
using System.Collections.Generic;

namespace DeaneBarker.Optimizely.Webhooks.Factories
{
    public interface IWebhookFactory
    {
        IEnumerable<Webhook> Generate(string action, IContent content);
        string Name { get; }
        Guid FactoryId { get; set; }
    }
}