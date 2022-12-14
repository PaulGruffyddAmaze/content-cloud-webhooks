using System.Net;

namespace DeaneBarker.Optimizely.Webhooks.HttpProcessors
{
    public interface IWebhookHttpProcessor
    {
        Task<WebhookAttempt> ProcessAsync(HttpRequestMessage request);
        //WebhookAttempt Process(HttpWebRequest request);
    }
}