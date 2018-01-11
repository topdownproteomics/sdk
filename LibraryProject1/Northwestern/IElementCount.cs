namespace TestLibNamespace.Northwestern
{
    /// <summary>
    /// Pair of element and its count.
    /// </summary>
    public interface IElementCount
    {
        /// <summary>
        /// Gets the element.
        /// </summary>
        IElement Element { get; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        int Count { get; }
    }
}