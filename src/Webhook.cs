using DeaneBarker.Optimizely.Webhooks.Blocks;
using DeaneBarker.Optimizely.Webhooks.Factories;
using DeaneBarker.Optimizely.Webhooks.Serializers;
using EPiServer.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DeaneBarker.Optimizely.Webhooks
{
    public class Webhook
    {
        private readonly List<WebhookAttempt> history = new List<WebhookAttempt>();

        public Guid Id { get; private set; }
        public DateTime Created { get; private set; }
        public IContent Content { get; private set; }
        public ContentReference ContentLink => Content?.ContentLink ?? ContentReference.EmptyReference;
        public Uri Target { get; private set; }
        public string Action { get; private set; }
        public bool Successful => History.Count != 0 && History.Last().Successful;
        public int AttemptCount => History.Count();
        public ReadOnlyCollection<WebhookAttempt> History => new ReadOnlyCollection<WebhookAttempt>(history.OrderBy(w => w.Executed).ToList());
        public IWebhookSerializer Serializer { get; private set; }
        public IWebhookFactory WebhookProfile { get; private set; }

        public Webhook(Uri target, string action, IWebhookFactory webhookProfile, IWebhookSerializer serializer, IContent content = null)
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
            Content = content;
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Action = action;
            WebhookProfile = webhookProfile;
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void AddHistory(WebhookAttempt attempt)
        {
            history.Add(attempt);
        }

        // This is just to make logging cleaner
        public string ToLogString()
        {
            if(Content != null)
            {
                return $"[{Action} / {Content.ContentLink} / {Id}]";
            }
            return $"[{Action} / {Id}]";
        }

    }

    // This exists just so we can have more careful control of how it's serialized (in particular, we don't want to serialize a full IContent object...)
    public class StorableWebhook
    {
        //TODO: These properties need setters to allow for deserialisation
        private Webhook webhook;

        public Guid id { get; set; }
        public Guid factoryBlockId { get; set; }
        public DateTime created { get; set; }
        public string target { get; set; }
        public ContentReference contentLink { get; set; }
        [JsonIgnore]
        public string contentName { get; set; }
        public string action { get; set; }

        public ReadOnlyCollection<WebhookAttempt> history { get; set; }

        public StorableWebhook()
        {

        }
        public StorableWebhook(Webhook _webhook)
        {
            webhook = _webhook;
            id = webhook.Id;
            factoryBlockId = (webhook.WebhookProfile as IContent)?.ContentGuid ?? Guid.Empty;
            created = webhook.Created;
            target = webhook.Target.AbsoluteUri;
            contentLink = webhook.ContentLink ?? ContentReference.EmptyReference;
            action = webhook.Action;
            history = webhook.History;
        }
    }

}