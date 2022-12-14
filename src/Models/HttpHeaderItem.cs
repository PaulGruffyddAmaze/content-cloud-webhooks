using EPiServer.Core;
using EPiServer.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.Models
{
    public class HttpHeaderItem
    {
        [RegularExpression("[a-zA-Z0-9-_]+", ErrorMessage = "Header name must be alphanumeric allowing - and _")]
        [Display(Name = "Header name", Order = 10)]
        public virtual string HeaderName { get; set; }

        [Display(Name = "Value", Order = 20)]
        public virtual string HeaderValue { get; set; }
    }

    [PropertyDefinitionTypePlugIn]
    public class HttpHeaderItemListProperty : PropertyList<HttpHeaderItem>
    {
    }
}
