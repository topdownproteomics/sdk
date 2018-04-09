namespace TopDownProteomics
{
    /// <summary>
    /// Default implementation of an entity cardinality.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IEntityCardinality{T}" />
    public class EntityCardinality<T> : IEntityCardinality<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCardinality{T}"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="count">The count.</param>
        public EntityCardinality(T entity, int count)
        {
            this.Entity = entity;
            this.Count = count;
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public T Entity { get; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count { get; }
    }
}