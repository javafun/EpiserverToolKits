using EPiServer;

namespace EpiserverExtensions.AspNet.PropertyExtensions
{
    public static class UrlExtensions
    {
        /// <summary>
        /// Checks if url property has value.
        /// </summary>
        /// <param name="url">Url to check <see cref="Url"/></param>
        /// <returns>True if url has value, otherwise false</returns>
        public static bool IsNull(this Url url)
        {
            return url != null && !url.IsEmpty();
        }
    }
}
