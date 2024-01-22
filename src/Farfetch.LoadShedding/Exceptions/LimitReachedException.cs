﻿using System;

namespace Farfetch.LoadShedding.Exceptions
{
    /// <summary>
    /// Represents an exception where a maximum limit defined was reached.
    /// </summary>
    public class LimitReachedException : Exception
    {
        /// <summary>
        /// Constructs the custom exception.
        /// </summary>
        /// <param name="message">Receives a message to be passed to the exception.</param>
        public LimitReachedException(string message) : base(message)
        {
        }
    }
}
