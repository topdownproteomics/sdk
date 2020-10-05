namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to Inputs element
	/// </summary>
	public class MzIdentMlInputs
	{
		/// <summary>
		/// Gets and sets the sourcefile
		/// </summary>
		public MzIdentMlSourceFile SourceFile { get; set; }

		/// <summary>
		/// Gets and sets the search database
		/// </summary>
		public MzIdentMlSearchDatabase SearchDatabase { get; set; }

		/// <summary>
		/// Gets and sets the spectradata
		/// </summary>
		public MzIdentMlSpectraData SpectraData { get; set; }
	}
}
