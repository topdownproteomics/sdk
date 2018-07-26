namespace TopDownProteomics
{
    /// <summary>
    /// Anything that has an integer ID and a text name.
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name.
        /// </summary>
        string Name { get; }
    }
}