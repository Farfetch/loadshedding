using System;

namespace Farfetch.LoadShedding.Tasks
{
    /// <summary>
    /// Extension methods of <see cref="Priority"/>
    /// </summary>
    public static class PriorityExtensions
    {
        /// <summary>
        /// Parses a string value into a <see cref="Priority"/> enum.
        /// The parser is case-insensetive and accepts hyphen. e.g. Normal, Non-Critival or CRITICAL.
        /// </summary>
        /// <param name="value">Priority string value to be parsed.</param>
        /// <returns>The Priority parsed value. Returns Priority.Normal if the value is invalid.</returns>
        public static Priority ParsePriority(this string value)
        {
            if (value is null)
            {
                return Priority.Normal;
            }

            var normalizedValue = value
                .Replace("-", string.Empty)
                .ToLowerInvariant();

            if (Enum.TryParse(normalizedValue, true, out Priority priority))
            {
                return priority;
            }

            return Priority.Normal;
        }

        /// <summary>
        /// Format a <see cref="Priority"/> enum to a string.
        /// </summary>
        /// <param name="priority">Priority value to be formatted.</param>
        /// <returns>The Priority format string.</returns>
        public static string FormatPriority(this Priority priority)
        {
            return priority.ToString().ToLowerInvariant();
        }
    }
}
