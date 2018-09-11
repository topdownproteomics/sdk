namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Provides elements.
    /// </summary>
    public interface IElementProvider
    {
        /// <summary>
        /// Gets the element by atomic number.
        /// </summary>
        /// <param name="atomicNumber">The atomic number.</param>
        /// <returns></returns>
        IElement GetElement(int atomicNumber);

        /// <summary>
        /// Gets the element by symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        IElement GetElement(string symbol);

        /// <summary>
        /// Gets the Carbon 13 isotopic element.
        /// </summary>
        /// <returns></returns>
        IElement GetCarbon13();
    }
}