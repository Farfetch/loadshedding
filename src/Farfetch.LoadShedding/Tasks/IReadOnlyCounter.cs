namespace Farfetch.LoadShedding.Tasks
{
    internal interface IReadOnlyCounter
    {
        int Count { get; }

        int Limit { get; }
    }
}
