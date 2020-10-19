using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the SpectrumIdentificationResult element
	/// </summary>
	public class MzIdentMlSpectrumIdentificationResult
	{
		/// <summary>
		/// Instantiates with required parameters
		/// </summary>
		/// <param name="id"></param>
		/// <param name="spectrumId"></param>
		/// <param name="inputSpectraDataId"></param>
		/// <param name="spectrumIdentificationItems"></param>
		public MzIdentMlSpectrumIdentificationResult(string id, string spectrumId, string inputSpectraDataId, List<MzIdentMlSpectrumIdentificationItem> spectrumIdentificationItems)
		{
			this.Id = id;
			this.SpectrumId = spectrumId;
			this.InputSpectraDataId = inputSpectraDataId;
			this.SpectrumIdentificationItems = spectrumIdentificationItems;
		}
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets and sets the spectrum id
		/// </summary>
		public string SpectrumId { get; }

		/// <summary>
		/// Gets and sets the input spectra data id
		/// </summary>
		public string InputSpectraDataId { get; }

		/// <summary>
		/// Gets and sets the spectrum identification items
		/// </summary>
		public List<MzIdentMlSpectrumIdentificationItem> SpectrumIdentificationItems { get; }

		/// <summary>
		/// Gets and sets the cvParams
		/// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }

		/// <summary>
		/// Gets and sets the userParams
		/// </summary>
		public List<MzIdentMlUserParam>? UserParams { get; set; }
	}
}
