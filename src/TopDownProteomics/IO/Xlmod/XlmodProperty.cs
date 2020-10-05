namespace TopDownProteomics.IO.Xlmod
{
    /// <summary>XLMOD property.</summary>
    public class XlmodProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XlmodProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="dataType">Type of the data.</param>
        public XlmodProperty(string name, string value, string dataType)
        {
            Name = name;
            Value = value;
            DataType = dataType;
        }

        /// <summary>The property name.</summary>
        public string Name { get; }

        /// <summary>The value of the property.</summary>
        public string Value { get; }

        /// <summary>The data type of the property.</summary>
        public string DataType { get; }
    }
}