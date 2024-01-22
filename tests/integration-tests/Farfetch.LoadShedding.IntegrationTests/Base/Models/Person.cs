using System.Diagnostics.CodeAnalysis;

namespace Farfetch.LoadShedding.IntegrationTests.Base.Models
{
    [ExcludeFromCodeCoverage]
    public class Person
    {
        public int Id { get; set; }

        public string? UserName { get; set; }

        public int Age { get; set; }
    }
}
