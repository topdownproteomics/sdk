namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to SpectraData element
	/// </summary>
	public class MzIdentMlSpectraData
	{
		/// <summary>
		/// Instantiates with required parameters
		/// </summary>
		/// <param name="id"></param>
		/// <param name="location"></param>
		/// <param name="spectrumIdFormat"></param>
		public MzIdentMlSpectraData(string id, string location, MzIdentMlCvParam spectrumIdFormat)
		{
			this.Id = id;
			this.Location = location;
			this.SpectrumIdFormat = spectrumIdFormat;
		}

		/// <summary>
		/// Gets the id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the location
		/// </summary>
		public string Location { get; }

		/// <summary>
		/// Gets and sets the file format
		/// </summary>
		public MzIdentMlCvParam? FileFormat { get; set; }

		/// <summary>
		/// Gets the spectrum id format
		/// </summary>
		public MzIdentMlCvParam SpectrumIdFormat { get; }
	}
}
