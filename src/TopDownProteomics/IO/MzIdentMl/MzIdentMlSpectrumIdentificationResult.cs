using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the SpectrumIdentificationResult element
	/// </summary>
	public class MzIdentMlSpectrumIdentificationResult
	{
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the spectrum id
		/// </summary>
		public string SpectrumId { get; set; }

		/// <summary>
		/// Gets and sets the input spectra data id
		/// </summary>
		public string InputSpectraDataId { get; set; }

		/// <summary>
		/// Gets and sets the spectrum identification items
		/// </summary>
		public List<MzIdentMlSpectrumIdentificationItem> SpectrumIdentificationItems { get; set; } = new List<MzIdentMlSpectrumIdentificationItem>();

		/// <summary>
		/// Gets and sets the parameters
		/// </summary>
		public List<MzIdentMlParam> Parameters { get; set; } = new List<MzIdentMlParam>();
	}
}
