using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to elements that are comprised of a single cvParam or userParam
	/// </summary>
	public class MzIdentMlCvOrUserParam
	{
		/// <summary>
		/// Instantiates with a cvParam
		/// </summary>
		/// <param name="cvParam"></param>
		public MzIdentMlCvOrUserParam(MzIdentMlCvParam cvParam)
		{
			this.CvParam = cvParam;
		}

		/// <summary>
		/// Instantiates with a userParam
		/// </summary>
		/// <param name="userParam"></param>
		public MzIdentMlCvOrUserParam(MzIdentMlUserParam userParam)
		{
			this.UserParam = userParam;
		}

		/// <summary>
		/// Gets the cvParam
		/// </summary>
		public MzIdentMlCvParam? CvParam { get; }

		/// <summary>
		/// Gets the userParam
		/// </summary>
		public MzIdentMlUserParam? UserParam { get; }
	}
}
