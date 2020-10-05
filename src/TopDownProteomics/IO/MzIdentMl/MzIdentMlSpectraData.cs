namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to SpectraData element
	/// </summary>
	public class MzIdentMlSpectraData
	{
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the location
		/// </summary>
		public string Location { get; set; }

		/// <summary>
		/// Gets and sets the file format
		/// </summary>
		public MzIdentMlParam FileFormat { get; set; }

		/// <summary>
		/// Gets and sets the spectrum id format
		/// </summary>
		public MzIdentMlParam SpectrumIdFormat { get; set; }
	}
}
