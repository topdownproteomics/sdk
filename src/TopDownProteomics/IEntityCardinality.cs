namespace TopDownProteomics
{
    /// <summary>
    /// The number of entities in a set or other grouping, as a property of that grouping.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityCardinality<T>
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        T Entity { get; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        int Count { get; }
    }
}