using DeaneBarker.Optimizely.Webhooks.Blocks;
using DeaneBarker.Optimizely.Webhooks.Factories;
using DeaneBarker.Optimizely.Webhooks.Serializers;
using EPiServer;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ILogger = EPiServer.Logging.ILogger;

namespace DeaneBarker.Optimizely.Webhooks
{
    public class SimpleWebhookFactoryProfile : IWebhookFactoryProfile
    {
        private readonly ILogger logger = LogManager.GetLogger(typeof(SimpleWebhookFactoryProfile));
        private readonly IContentLoader _contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

        public Uri Target { get; set; }
        public ICollection<Type> IncludeTypes { get; set; } = new List<Type>();
        public ICollection<Type> ExcludeTypes { get; set; } = new List<Type>();
        public ICollection<string> IncludeActions { get; set; } = new List<string>();
        public ICollection<string> ExcludeActions { get; set; } = new List<string>();
        public IWebhookSerializer Serializer { get; set; }
        public ContentReference StartingPoint { get; set; } = ContentReference.RootPage;

        public SimpleWebhookFactoryProfile(string target)
        {
            Target = new Uri(target);
        }

        public SimpleWebhookFactoryProfile(Uri target)
        {
            Target = target;
        }

        public IEnumerable<Webhook> Process(string action, IWebhookFactory webhookProfile, IContent content = null)
        {
            if(content == null)
            {
                logger.Debug($"Webhook not produced. Content is null.");
                return null;
            }

            if (!_contentLoader.GetAncestors(content.ContentLink).Any(x => x.ContentLink.ToReferenceWithoutVersion().Equals(StartingPoint.ToReferenceWithoutVersion())))
            {
                logger.Debug($"Webhook not produced. Content is not under starting point.");
                return null;
            }

            logger.Debug($"Executing factory profile on content {content.ContentLink} bearing action {action.Quoted()}");

            if (Target == null)
            {
                logger.Debug("Webhook not produced. Target not set");
                return null;
            }

            var type = content.GetType().BaseType;

            if (ExcludeTypes.Contains(type))
            {
                logger.Debug($"Webhook not produced. {type} is an excluded type");
                return null;
            }

            if (ExcludeActions.Contains(action))
            {
                logger.Debug($"Webhook not produced. {action} is an excluded action");
                return null;
            }

            if (IncludeTypes.Any() && !IncludeTypes.Contains(type))
            {
                logger.Debug($"Webhook not produced. {type} is not an included type");
                return null;
            }

            if (IncludeActions.Any() && !IncludeActions.Contains(action))
            {
                logger.Debug($"Webhook not produced. {action} is not an included action");
                return null;
            }

            var settings = ServiceLocator.Current.GetInstance<WebhookSettings>();
            return new List<Webhook>()
            {
                new Webhook(Target, action, webhookProfile, Serializer ?? settings.DefaultSerializer, content)
            };
        }
    }
}