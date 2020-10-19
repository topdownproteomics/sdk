namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to Measure elements within the FragmentationTable
	/// </summary>
	public class MzIdentMlFragmentationMeasure
	{
		/// <summary>
		/// Instantiates with required id and cvParam
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="cvParam">The cvParam</param>
		public MzIdentMlFragmentationMeasure(string id, MzIdentMlCvParam cvParam)
		{
			this.Id = id;
			this.Measure = cvParam;
		}
		/// <summary>
		/// Gets the Id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the measure
		/// </summary>
		public MzIdentMlCvParam Measure { get; }
	}
}
