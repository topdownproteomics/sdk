using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	public class IonType
	{
		public int[] Indices { get; set; }
		public int Charge { get; set; }
		public double[] Mzs { get; set; }
		public double[] Intensities { get; set; }
		public double[] MzError { get; set; }
		public List<CvParam> CvParams { get; set; } = new List<CvParam>();
	}
}
