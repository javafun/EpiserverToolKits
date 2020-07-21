using System;


namespace Extensions.Core
{
    public static class StringExtensions
    {
        /// <summary>
        /// Compares 2 strings with invariant culture and case ignored.
        /// </summary>
        /// <param name="value">compare</param>
        /// <param name="valueToCompare">compareTo</param>
        /// <returns>True if strings are same, otherwise false.</returns>
        public static bool InvariantEquals(this string value, string valueToCompare)
        {
            if (string.IsNullOrWhiteSpace(valueToCompare))
                return false;

            return value.Equals(valueToCompare, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
