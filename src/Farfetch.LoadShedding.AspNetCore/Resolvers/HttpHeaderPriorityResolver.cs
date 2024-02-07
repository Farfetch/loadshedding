using System;
using System.Threading.Tasks;
using Farfetch.LoadShedding.Tasks;
using Microsoft.AspNetCore.Http;

namespace Farfetch.LoadShedding.AspNetCore.Resolvers
{
    internal class HttpHeaderPriorityResolver : IPriorityResolver
    {
        internal const string DefaultPriorityHeaderName = "X-Priority";

        private const string Separator = "-";

        private readonly string _headerName;

        public HttpHeaderPriorityResolver()
            : this(DefaultPriorityHeaderName)
        {
        }

        public HttpHeaderPriorityResolver(string headerName)
        {
            this._headerName = headerName;
        }

        public Task<Priority> ResolveAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(this._headerName, out var values))
            {
                return Task.FromResult(Priority.Normal);
            }

            var normalizedValue = this.NormalizeHeaderValue(values);

            if (!Enum.TryParse(normalizedValue, true, out Priority priority))
            {
                priority = Priority.Normal;
            }

            return Task.FromResult(priority);
        }

        private string NormalizeHeaderValue(string headerValue)
        {
            return headerValue
                .Replace(Separator, string.Empty)
                .ToLower();
        }
    }
}
