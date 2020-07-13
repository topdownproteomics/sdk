namespace TopDownProteomics
{
    /// <summary>Anything that has a text ID and a text name.</summary>
    public interface IIdentifiable
    {
        /// <summary>The identifier.</summary>
        string Id { get; }

        /// <summary>The name.</summary>
        string Name { get; }
    }
}