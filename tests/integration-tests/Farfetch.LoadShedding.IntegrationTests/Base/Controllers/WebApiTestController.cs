using System.Diagnostics.CodeAnalysis;
using Farfetch.LoadShedding.AspNetCore.Attributes;
using Farfetch.LoadShedding.IntegrationTests.Base.Models;
using Farfetch.LoadShedding.Tasks;
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
        [EndpointPriority(Priority.Critical)]
        public async Task<IActionResult> GetPeopleAsync()
        {
            await Task.Delay(500);

            return this.Ok(new[]
            {
                new Person
                {
                    Id = 1,
                    Age = 18,
                    UserName = "john.doe",
                },
            });
        }

        [HttpDelete]
        [Route("people")]
        [EndpointPriority(Priority.Critical)]
        public Task DeletePeopleAsync()
        {
            return Task.Delay(500);
        }
    }
}
