using DeaneBarker.Optimizely.Webhooks;
using DeaneBarker.Optimizely.Webhooks.Blocks;
using DeaneBarker.Optimizely.Webhooks.Helpers;
using DeaneBarker.Optimizely.Webhooks.Serializers;
using System.Net;

namespace DeaneBarker.Optimizely.Serializers
{
    public class GetWebhookSerializer : IWebhookSerializer
    {
        public HttpRequestMessage Serialize(Webhook webhook)
        {
            var request = new WebRequestBuilder()
                .AsGet()
                .ToUrl(webhook.Target)
                .WithQuerystringArg("action", webhook.Action)
                .WithQuerystringArg("id", webhook.ContentLink?.ID.ToString() ?? "0");
                
            if (webhook.WebhookProfile is WebhookFactoryBlock webhookFactoryBlock && (webhookFactoryBlock.CustomHeaders?.Any() ?? false))
            {
                // add headers
                foreach (var header in webhookFactoryBlock.CustomHeaders)
                {
                    request = request.WithHeader(header.HeaderName, header.HeaderValue);
                }
            }
            return request.Build();
        }
    }

}