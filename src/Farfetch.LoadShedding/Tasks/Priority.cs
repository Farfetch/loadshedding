namespace Farfetch.LoadShedding.Tasks
{
    /// <summary>
    /// Task priorities.
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// Priority as Critical.
        /// </summary>
        Critical = 0,

        /// <summary>
        /// Priority as Normal.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Priority as Non Critical.
        /// </summary>
        NonCritical = 2,
    }
}
