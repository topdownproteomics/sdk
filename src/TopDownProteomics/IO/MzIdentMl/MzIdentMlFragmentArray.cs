namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the FragmentArray element
	/// </summary>
	public class MzIdentMlFragmentArray
	{
		/// <summary>
		/// Instantiates with required parameters
		/// </summary>
		/// <param name="measure"></param>
		/// <param name="values"></param>
		public MzIdentMlFragmentArray(MzIdentMlFragmentationMeasure measure, double[] values)
		{
			this.Measure = measure;
			this.Values = values;
		}

		/// <summary>
		/// Gets the measure
		/// </summary>
		public MzIdentMlFragmentationMeasure Measure { get; }

		/// <summary>
		/// Gets the values
		/// </summary>
		public double[] Values { get; }
	}
}
