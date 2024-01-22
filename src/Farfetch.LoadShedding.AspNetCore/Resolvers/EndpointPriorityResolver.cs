using System.Threading.Tasks;
using Farfetch.LoadShedding.AspNetCore.Attributes;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Farfetch.LoadShedding.AspNetCore.Resolvers
{
    internal class EndpointPriorityResolver : IPriorityResolver
    {
        public Task<Priority> ResolveAsync(HttpContext context)
        {
            var priority = this.GetEndpointPriority(context);

            return Task.FromResult(priority);
        }

        private Priority GetEndpointPriority(HttpContext context)
        {
            return context?
               .Features?
               .Get<IEndpointFeature>()?
               .Endpoint?
               .Metadata?
               .GetMetadata<EndpointPriorityAttribute>()?
               .Priority ?? Priority.Normal;
        }
    }
}
