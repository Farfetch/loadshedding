namespace Farfetch.LoadShedding.Tasks
{
    /// <summary>
    /// Task priorities.
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// Priority as critical.
        /// </summary>
        Critical = 0,

        /// <summary>
        /// Priority as normal.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Priority as critical.
        /// </summary>
        NonCritical = 2,
    }
}
