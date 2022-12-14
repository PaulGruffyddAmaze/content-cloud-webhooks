using DeaneBarker.Optimizely.Webhooks.Blocks;
using DeaneBarker.Optimizely.Webhooks.Helpers;
using EPiServer.Core;
using EPiServer.Logging;
using System.Net;
using System.Xml.Linq;
using ILogger = EPiServer.Logging.ILogger;

namespace DeaneBarker.Optimizely.Webhooks.Serializers
{
    public class PostXmlWebhookSerializer : IWebhookSerializer
    {
        private readonly ILogger logger = LogManager.GetLogger(typeof(PostXmlWebhookSerializer));

        public HttpRequestMessage Serialize(Webhook webhook)
        {           
            var requestBody = SerializeIContentAsXml(webhook.Content);
            logger.Debug($"Serialized content {webhook.Content.ContentLink} into {requestBody.Length} byte(s). ID: {webhook.Id}");

            var request = new WebRequestBuilder()
                .AsPost()
                .ToUrl(webhook.Target)
                .WithBody(requestBody, "text/xml")
                .WithQuerystringArg("action", webhook.Action);
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

        protected string SerializeIContentAsXml(IContent content)
        {
            var doc = new XDocument(new XElement("object", content.Property.Select(p => getElement(p))));
            return doc.ToString();

            XElement getElement(PropertyData prop)
            {
                var elementName = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
                var typeName = prop.Type.ToString();

                var element = new XElement(elementName, new XAttribute("type", typeName));

                if (prop.Value == null)
                    return element;

                if (prop.Value.ToString().Contains("<"))
                {
                    element.Add(new XCData(prop.Value.ToString()));
                    return element;
                }

                element.Value = prop.Value.ToString();
                return element;
            }
        }
    }
}