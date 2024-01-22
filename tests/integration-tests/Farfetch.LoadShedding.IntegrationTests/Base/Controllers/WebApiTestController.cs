using System.Diagnostics.CodeAnalysis;
using Farfetch.LoadShedding.AspNetCore.Attributes;
using Farfetch.LoadShedding.IntegrationTests.Base.Models;
using Microsoft.AspNetCore.Mvc;

namespace Farfetch.LoadShedding.IntegrationTests.Base.Controllers
{
    [ApiController]
    [Route("api")]
    [ExcludeFromCodeCoverage]
    public class WebApiTestController : ControllerBase
    {
        [HttpGet]
        [Route("people")]
        [EndpointPriority(Tasks.Priority.Critical)]
        public async Task<IActionResult> GetPeopleAsync()
        {
            await Task.Delay(500);

            return this.Ok(new[]
            {
                new Person
                {
                    Id = 1,
                    Age = 18,
                    UserName = "john.doe"
                }
            });
        }
    }
}
