using System.Net;

namespace DeaneBarker.Optimizely.Webhooks.Serializers
{
    public interface IWebhookSerializer
    {
        HttpRequestMessage Serialize(Webhook webhook);
    }
}