using DeaneBarker.Optimizely.Serializers;
using DeaneBarker.Optimizely.Webhooks.Blocks;
using DeaneBarker.Optimizely.Webhooks.Factories;
using DeaneBarker.Optimizely.Webhooks.HttpProcessors;
using DeaneBarker.Optimizely.Webhooks.Queues;
using DeaneBarker.Optimizely.Webhooks.Serializers;
using DeaneBarker.Optimizely.Webhooks.Stores;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;
using Webhooks.Models;

namespace DeaneBarker.Optimizely.Webhooks
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class WebhooksInit : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            // This manages the entire webhook process
            // All other services are injected into this one
            // This is where the event handlers are located
            context.Services.AddSingleton<IWebhookManager, WebhookManager>();

            // This makes the actual HTTP call
            // I broke this out to its own service so it could be mocked -- I needed a way to test if the call failed
            context.Services.AddSingleton<IWebhookHttpProcessor, WebhookHttpProcessor>();
            //context.Services.AddSingleton<IWebhookHttpProcessor, UnstableWebhookHttpProcessor>();

            // This persists the webhook and its history to some data source
            context.Services.AddSingleton<IWebhookStore, FileSystemWebhookStore>();

            // This holds the pending webhooks and manages the process that works them
            context.Services.AddSingleton<IWebhookQueue, InMemoryWebhookQueue>();

            // Holds various settings
            context.Services.AddSingleton<WebhookSettings, WebhookSettings>();

            // Executes all the IWebhookFactoryProfiles to produce webhooks
            context.Services.AddSingleton<WebhookFactoryManager, WebhookFactoryManager>();
        }

        public void Initialize(InitializationEngine context)
        {
            //var contentRepository = context.Locate.ContentRepository();
            //var rootContent = contentRepository.GetChildren<WebhookRoot>(ContentReference.RootPage);
            //if (rootContent == null || !rootContent.Any())
            //{
            //    var templatesRoot = contentRepository.GetDefault<WebhookRoot>(ContentReference.RootPage);
            //    templatesRoot.Name = "Webhook root";
            //    WebhookFactoryBlock.WebhookFactoryBlockFolderId = contentRepository.Publish(templatesRoot, AccessLevel.NoAccess).ID;

            //    //TODO: Set default permissions?
            //}
            //else
            //{
            //    WebhookFactoryBlock.WebhookFactoryBlockFolderId = rootContent.First().ContentLink.ID;
            //}
            var contentRootService = context.Locate.Advanced.GetInstance<ContentRootService>();
            contentRootService.Register<ContentFolder>("Webhooks", new Guid("871f2aed-880a-48d2-a325-58c2fd49b4e2"), ContentReference.RootPage);
            WebhookFactoryBlock.WebhookFactoryBlockFolderId = contentRootService.Get("Webhooks").ID;
            WebhookFactoryBlock.AvailableSerializers.Add("GET (with ID)", typeof(GetWebhookSerializer));
            WebhookFactoryBlock.AvailableSerializers.Add("POST (as JSON)", typeof(PostJsonWebhookSerializer));
            WebhookFactoryBlock.AvailableSerializers.Add("POST (as XML)", typeof(PostXmlWebhookSerializer));

            var webhookManager = ServiceLocator.Current.GetInstance<IWebhookManager>();

            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishedContent += webhookManager.QueuePublishedWebhook;
            contentEvents.MovedContent += webhookManager.QueueMovedWebhook;
            contentEvents.DeletedContent += webhookManager.QueueDeletedWebhook;
        }

        public void Uninitialize(InitializationEngine context)
        {
            var webhookQueue = ServiceLocator.Current.GetInstance<IWebhookQueue>(); // This is registered as a Singleton, so this should get the same instance where the thread were defined
            webhookQueue.Dispose();
        }
    }
}