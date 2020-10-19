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
		/// <param name="measureId"></param>
		/// <param name="values"></param>
		public MzIdentMlFragmentArray(string measureId, double[] values)
		{
			this.MeasureId = measureId;
			this.Values = values;
		}

		/// <summary>
		/// Gets the measure Id
		/// </summary>
		public string MeasureId { get; }

		/// <summary>
		/// Gets the values
		/// </summary>
		public double[] Values { get; }
	}
}
