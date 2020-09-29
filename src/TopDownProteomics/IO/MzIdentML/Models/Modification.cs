using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class Modification
	{
		public Modification(int oneBasedLocationIndex, double monoisotopicMassDelta, List<CvParam> cvParams)
		{
			this._oneBasedLocationIndex = oneBasedLocationIndex;
			this._monoisotopicMassDelta = monoisotopicMassDelta;
			this._cvParams = cvParams;

			this.CreateProFormaDescriptor();
			this.CreateProFormaTag();
		}

		private readonly int _oneBasedLocationIndex;
		private readonly double _monoisotopicMassDelta;
		private readonly List<CvParam> _cvParams;
		public ProFormaDescriptor ProFormaDescriptor { get; set; }
		public ProFormaTag ProFormaTag { get; set; }
		public ModLocationType ModLocationType { get; set; }

		private void CreateProFormaDescriptor()
		{
			// if the mod doesn't have CVParams, use the mass if it's > 0
			string modName;
			if (this._cvParams.Count == 0)
			{
				if (this._monoisotopicMassDelta > 0)
					modName = this._monoisotopicMassDelta.ToString();
				else
					modName = "unknownMod";
			}
			else
			{
				// if there's a mod Accession use that, otherwise use Name
				var cvParam = this._cvParams.First();
				if (!string.IsNullOrEmpty(cvParam.Accession))
					modName = cvParam.Accession;
				else if (!string.IsNullOrEmpty(cvParam.Name))
					modName = cvParam.Name;
				else
					modName = "unknownMod";
			}

			this.ProFormaDescriptor = new ProFormaDescriptor(modName);
		}

		private void CreateProFormaTag()
		{
			this.ProFormaTag = new ProFormaTag(this._oneBasedLocationIndex - 1, new[] { this.ProFormaDescriptor });
		}

		public ModLocationType GetModLocationType(int sequenceLength)
        {
			if (this._oneBasedLocationIndex == 0)
				return ModLocationType.NTerminal;
			else if (this._oneBasedLocationIndex == sequenceLength)
				return ModLocationType.CTerminal;
			else
				return ModLocationType.Internal;
		}
	}

	public enum ModLocationType
    {
		NTerminal,
		Internal,
		CTerminal
    }
}
