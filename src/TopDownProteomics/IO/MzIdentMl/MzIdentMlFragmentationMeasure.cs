namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to Measure elements
	/// </summary>
	public class MzIdentMlFragmentationMeasure
	{
		/// <summary>
		/// Gets and sets the Id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the measure
		/// </summary>
		public MzIdentMlParam Measure { get; set; }
	}
}
