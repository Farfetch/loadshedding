using System;
using Farfetch.LoadShedding.Tasks;

namespace Farfetch.LoadShedding.AspNetCore.Attributes
{
    public class EndpointPriorityAttribute : Attribute
    {
        public EndpointPriorityAttribute(Priority priority)
        {
            Priority = priority;
        }

        internal Priority Priority { get; }
    }
}
