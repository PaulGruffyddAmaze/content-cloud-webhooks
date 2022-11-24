using DeaneBarker.Optimizely.Webhooks.Blocks;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webhooks.UI
{
    [ServiceConfiguration(typeof(IContentRepositoryDescriptor))]
    public class WebhookContentRepositoryDescriptor : ContentRepositoryDescriptorBase
    {
        private ContentReference _root;

        public WebhookContentRepositoryDescriptor(ContentRootService contentRootService)
        {
            _root = contentRootService.Get("Webhooks");
        }

        public override string Key => "webhooks";
        public override string Name => "Webhooks";
        public override IEnumerable<ContentReference> Roots
        {
            get 
            {
                if (ContentReference.IsNullOrEmpty(_root))
                {
                    _root = ServiceLocator.Current.GetInstance<ContentRootService>().Get("Webhooks");
                }
                return new[] { _root }; }
        }
        public override IEnumerable<Type> ContainedTypes => new[] { typeof(WebhookFactoryBlock), typeof(ContentFolder) };

        public override IEnumerable<Type> CreatableTypes => new[] { typeof(WebhookFactoryBlock) };
        public override IEnumerable<Type> LinkableTypes => new[] { typeof(WebhookFactoryBlock) };
        public override IEnumerable<Type> MainNavigationTypes => new[] { typeof(ContentFolder), typeof(WebhookFactoryBlock) };
    }
}
