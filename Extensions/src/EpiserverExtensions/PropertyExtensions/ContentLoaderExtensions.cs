using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Linq;

namespace EpiserverExtensions.Core.PropertyExtensions
{
    public static class ContentLoaderExtensions
    {
        public static IEnumerable<T> GetPublishedChildren<T>(this IContentLoader contentLoader, ContentReference contentLink) where T : IContent
        {
            var publishedStateAccessor = ServiceLocator.Current.GetInstance<IPublishedStateAssessor>();

            return contentLoader.GetChildren<T>(contentLink).Where(x => publishedStateAccessor.IsPublished(x, PublishedStateCondition.None));
        }
    }
}
