﻿using EPiServer.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ILogger = EPiServer.Logging.ILogger;

namespace DeaneBarker.Optimizely.Webhooks.Stores
{
    public class InMemoryWebhookStore : IWebhookStore
    {
        private readonly ILogger logger = LogManager.GetLogger(typeof(InMemoryWebhookStore));

        private List<Webhook> webhooks = new List<Webhook>();

        public void Store(Webhook webhook)
        {
            logger.Debug($"Storing webhook {webhook.ToLogString()}");
            webhooks.Add(webhook);
        }

        public IEnumerable<StorableWebhook> GetResultsByFactoryId(Guid id)
        {
            return webhooks.Where(x => x.WebhookProfile.FactoryId.Equals(id)).Select(x => new StorableWebhook(x)).OrderByDescending(x => x.created);
        }
    }
}