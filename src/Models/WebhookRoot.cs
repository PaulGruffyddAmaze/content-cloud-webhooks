using DeaneBarker.Optimizely.Webhooks.Blocks;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.Models
{
    [ContentType(
        DisplayName = "Webhook root",
        Description = "The root folder for webhook definitions",
        GUID = "0b25aad5-c916-4a03-b6fe-ff38d90f4d29")]
    //[AvailableContentTypes(
    //    Availability = Availability.Specific,
    //    IncludeOn = new[] { typeof(WebhookFactoryBlock) })]
    public class WebhookRoot : ContentFolder { }
}
