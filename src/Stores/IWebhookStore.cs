namespace DeaneBarker.Optimizely.Webhooks.Stores
{
    public interface IWebhookStore
    {
        void Store(Webhook webhook);
        IEnumerable<StorableWebhook> GetResultsByFactoryId(Guid id);
    }
}