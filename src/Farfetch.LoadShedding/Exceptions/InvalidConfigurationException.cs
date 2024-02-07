using System;

namespace Farfetch.LoadShedding.Exceptions
{
    /// <summary>
    /// Represents an exception where some configuration is invalid.
    /// </summary>
    public class InvalidConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// Constructs the custom exception.
        /// </summary>
        /// <param name="message">Receives a message to be passed to the exception.</param>
        public InvalidConfigurationException(string message)
            : base(message)
        {
        }
    }
}
