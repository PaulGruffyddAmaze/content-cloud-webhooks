
using DeaneBarker.Optimizely.Webhooks.Blocks;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.Logging;
using Microsoft.AspNetCore;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using ILogger = EPiServer.Logging.ILogger;

namespace DeaneBarker.Optimizely.Webhooks.Stores
{
    public class FileSystemWebhookStore : IWebhookStore
    {
        private readonly ILogger logger = LogManager.GetLogger(typeof(FileSystemWebhookStore));
        private readonly IContentLoader _contentLoader;

        public static string StorePath { get; set; }

        public FileSystemWebhookStore(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public void Store(Webhook webhook)
        {
            logger.Debug($"Storing webhook {webhook.ToLogString()}");

            if (StorePath == null)
            {
                logger.Error("StorePath value is not set.");
                return;
            }
            var storePath = StorePath;
            if (webhook.WebhookProfile is IContent webhookFactoryBlock)
            {
                storePath = Path.Combine(StorePath, webhookFactoryBlock.ContentGuid.ToString());
            }

            if(!Directory.Exists(storePath))
            {
                Directory.CreateDirectory(storePath);
                logger.Debug($"Created directory at {storePath}");
            }

            var fullPath = Path.Combine(storePath, GetFileName(webhook));

            var serializer = new Newtonsoft.Json.JsonSerializer();
            var sw = new StringWriter();
            serializer.Serialize(sw, new StorableWebhook(webhook));
            var content = sw.ToString();

            File.WriteAllText(fullPath, content);
            logger.Debug($"Wrote {content.Length} character(s) to {fullPath.Quoted()} {webhook.ToLogString()}");
        }

        public IEnumerable<StorableWebhook> GetResultsByFactoryId(Guid id)
        {
            logger.Debug($"Retrieving webhook results for {id}");
            var rtn = new List<StorableWebhook>();

            if (StorePath == null)
            {
                logger.Error("StorePath value is not set.");
                return rtn;
            }

            if (_contentLoader.TryGet(id, out IContent content))
            {
                var storePath = Path.Combine(StorePath, content.ContentGuid.ToString());
                
                if (!Directory.Exists(storePath))
                {
                    return rtn;
                }
                foreach (var filePath in Directory.GetFiles(storePath))
                {
                    try
                    {
                        var file = File.ReadAllText(filePath);
                        if (!string.IsNullOrEmpty(file))
                        {
                            rtn.Add(JsonConvert.DeserializeObject<StorableWebhook>(file));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Unable to read file {filePath}", ex);
                    }
                }
            }
            return rtn.OrderByDescending(x => x.created);
        }

        protected string GetFileName(Webhook webhook)
        {
            return string.Concat(webhook.Id.ToString(), ".json");
        }
    }
}