using DeaneBarker.Optimizely.Serializers;
using DeaneBarker.Optimizely.Webhooks.Blocks;
using DeaneBarker.Optimizely.Webhooks.Serializers;
using DeaneBarker.Optimizely.Webhooks;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using EPiServer;

namespace Webhooks.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebhooks(this IApplicationBuilder app)
        {
            var contentRootService = app.ApplicationServices.GetRequiredService<ContentRootService>();
            var contentLoader = app.ApplicationServices.GetRequiredService<IContentLoader>();
            contentRootService.Register<ContentFolder>("Webhooks", new Guid("871f2aed-880a-48d2-a325-58c2fd49b4e2"), ContentReference.RootPage);
            WebhookFactoryBlock.WebhookFactoryBlockFolderId = contentRootService.Get("Webhooks").ID;
            WebhookFactoryBlock.AvailableSerializers.Add("GET (with ID)", typeof(GetWebhookSerializer));
            WebhookFactoryBlock.AvailableSerializers.Add("POST (as JSON)", typeof(PostJsonWebhookSerializer));
            WebhookFactoryBlock.AvailableSerializers.Add("POST (as XML)", typeof(PostXmlWebhookSerializer));

            var webhookManager = app.ApplicationServices.GetRequiredService<IWebhookManager>();

            var contentEvents = app.ApplicationServices.GetRequiredService<IContentEvents>();
            contentEvents.PublishedContent += webhookManager.QueuePublishedWebhook;
            contentEvents.MovedContent += webhookManager.QueueMovedWebhook;
            contentEvents.DeletedContent += webhookManager.QueueDeletedWebhook;


            WebhookFactoryBlock.RegisterFactories();

            // If anything in the Webhooks folder is published, re-register all the blocks
            contentEvents.PublishedContent += (object s, ContentEventArgs e) =>
            {
                if (e.Content is WebhookFactoryBlock)
                {
                    WebhookFactoryBlock.RegisterFactories();
                }

            };

            contentEvents.MovedContent += (object s, ContentEventArgs e) =>
            {

                if (e.Content is WebhookFactoryBlock)
                {
                    WebhookFactoryBlock.RegisterFactories();
                }

            };

            return app;
        }
    }
}
