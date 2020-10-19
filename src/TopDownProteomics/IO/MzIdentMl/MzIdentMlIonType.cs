using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the IonType element
	/// </summary>
	public class MzIdentMlIonType
	{
		/// <summary>
		/// Instantiates with required cvParams
		/// </summary>
		/// <param name="cvParams"></param>
		/// <param name="charge"></param>
		public MzIdentMlIonType(List<MzIdentMlCvParam> cvParams, int charge)
		{
			this.CvParams = cvParams;
			this.Charge = charge;
		}

		/// <summary>
		/// Gets the cvParams
		/// </summary>
		public List<MzIdentMlCvParam> CvParams { get; }

		/// <summary>
		/// Gets and sets the indices
		/// </summary>
		public int[]? Indices { get; set; }

		/// <summary>
		/// Gets and sets the charge
		/// </summary>
		public int Charge { get; set; }

		/// <summary>
		/// Gets and sets the fragment arrays
		/// </summary>
		public List<MzIdentMlFragmentArray>? FragmentArrays { get; set; }

		/// <summary>
		/// Gets the userParams
		/// </summary>
		public List<MzIdentMlUserParam>? UserParams { get; set; }
	}
}
