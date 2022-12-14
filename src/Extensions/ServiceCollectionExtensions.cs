using DeaneBarker.Optimizely.Webhooks.Factories;
using DeaneBarker.Optimizely.Webhooks.HttpProcessors;
using DeaneBarker.Optimizely.Webhooks.Queues;
using DeaneBarker.Optimizely.Webhooks.Stores;
using DeaneBarker.Optimizely.Webhooks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebhooks(this IServiceCollection services)
        {
            // This manages the entire webhook process
            // All other services are injected into this one
            // This is where the event handlers are located
            services.AddSingleton<IWebhookManager, WebhookManager>();

            // This makes the actual HTTP call
            // I broke this out to its own service so it could be mocked -- I needed a way to test if the call failed
            services.AddSingleton<IWebhookHttpProcessor, WebhookHttpProcessor>();
            //context.Services.AddSingleton<IWebhookHttpProcessor, UnstableWebhookHttpProcessor>();

            // This persists the webhook and its history to some data source
            services.AddSingleton<IWebhookStore, FileSystemWebhookStore>();

            // This holds the pending webhooks and manages the process that works them
            services.AddSingleton<IWebhookQueue, InMemoryWebhookQueue>();

            // Holds various settings
            services.AddSingleton<WebhookSettings, WebhookSettings>();

            // Executes all the IWebhookFactoryProfiles to produce webhooks
            services.AddSingleton<WebhookFactoryManager, WebhookFactoryManager>();

            return services;
        }
    }
}
