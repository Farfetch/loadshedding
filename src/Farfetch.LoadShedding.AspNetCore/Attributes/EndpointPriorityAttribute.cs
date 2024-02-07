using System;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.AspNetCore.Attributes
{
    public class EndpointPriorityAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointPriorityAttribute"/> class.
        /// </summary>
        /// <param name="priority"></param>
        public EndpointPriorityAttribute(Priority priority)
        {
            Priority = priority;
        }

        internal Priority Priority { get; }
    }
}
