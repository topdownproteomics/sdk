using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to elements that are collections of cvParams and userParams
	/// </summary>
	public class MzIdentMlParamCollection
	{
		/// <summary>
		/// Instantiates with both cvParams and userParams
		/// </summary>
		/// <param name="cvParams"></param>
		/// <param name="userParams"></param>
		public MzIdentMlParamCollection(List<MzIdentMlCvParam> cvParams, List<MzIdentMlUserParam> userParams)
		{
			this.CvParams = cvParams;
			this.UserParams = userParams;
		}

		/// <summary>
		/// Instantiates with cvParams
		/// </summary>
		/// <param name="cvParams"></param>
		public MzIdentMlParamCollection(List<MzIdentMlCvParam> cvParams)
		{
			this.CvParams = cvParams;
		}

		/// <summary>
		/// Instantiates with userParams
		/// </summary>
		/// <param name="userParams"></param>
		public MzIdentMlParamCollection(List<MzIdentMlUserParam> userParams)
		{
			this.UserParams = userParams;
		}

		/// <summary>
		/// Gets the cvParams
		/// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; }

		/// <summary>
		/// Gets the userParams
		/// </summary>
		public List<MzIdentMlUserParam>? UserParams { get; }
	}
}
