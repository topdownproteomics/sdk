namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to SourceFile elements
	/// </summary>
	public class MzIdentMlSourceFile
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
	}
}
