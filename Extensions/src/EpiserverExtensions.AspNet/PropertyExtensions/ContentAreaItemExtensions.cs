using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EpiserverExtensions.AspNet.PropertyExtensions
{
    public static class ContentAreaItemExtensions
    {
        /// <summary>
        /// Returns all the content items of <typeparamref name="T"/> from <paramref name="contentAreaItems"/>
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IContentData"/> items to extract.</typeparam>
        /// <param name="contentAreaItems">The <see cref="IEnumerable{ContentAreaItem}"/> to extract <see cref="IContent"/> instances of type <typeparamref name="T"/> from.</param>
        /// <param name="language">The <see cref="CultureInfo"/> to use. If none is specified, current culture will be used.</param>
        /// <param name="contentLoader">The <see cref="IContentLoader"/> to use</param>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all <see cref="IContent"/> items found in the <paramref name="contentAreaItems"/> collection.</returns>        
        public static IEnumerable<T> GetContentItems<T>(this IEnumerable<ContentAreaItem> contentAreaItems, CultureInfo language = null,
            IContentLoader contentLoader = null)
            where T : IContentData
        {
            if (contentAreaItems == null)
            {
                return Enumerable.Empty<T>();
            }

            if (contentLoader == null)
            {
                contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            }

            if (language == null)
            {
                language = LanguageSelector.AutoDetect().Language;
            }


            var items = contentLoader.GetItems(contentAreaItems.Select(i => i.ContentLink), language).OfType<T>();

            var publishedFilter = new FilterPublished();
            var accessFilter = new FilterAccess();

            var filterItems = items.OfType<IContent>().Where(x => !publishedFilter.ShouldFilter(x) && !accessFilter.ShouldFilter(x));

            return filterItems.OfType<T>();
        }
    }
}
