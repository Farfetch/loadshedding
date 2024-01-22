﻿using System;

namespace Farfetch.LoadShedding.Exceptions
{
    /// <summary>
    ///  Represents an exception where the queue timeout defined was reached.
    /// </summary>
    public class QueueTimeoutException : TimeoutException
    {
        /// <summary>
        /// Constructs the custom exception.
        /// </summary>
        /// <param name="timeout">The current queue timeout.</param>
        public QueueTimeoutException(int timeout)
            : base($"The maximum queue timeout of {timeout} was reached.")
        {
        }
    }
}
