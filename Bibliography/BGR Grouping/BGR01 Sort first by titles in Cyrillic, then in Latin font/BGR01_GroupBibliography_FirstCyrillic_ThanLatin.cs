//BGR01
//21.06.2018 

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Citations;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;

namespace SwissAcademic.Citavi.Comparers
{
	public class CustomCitationComparer
		:
		ICustomCitationComparerMacro
	{
		public int Compare(Citation x, Citation y)
		{
			/*
				This is an example of a custom sort macro that sorts all references of type 'internet document' on top of the bibliography.
				The internet documents themselves are sorted according to a different logic than the rest of the cited documents.
				Return values:
				0:					x is considered the same as y sorting-wise, so we cannot tell a difference based on the algorithm below
				> 0 (positive):		x should go after y, x is greater than y
				< 0 (negative):		x should go before y, x is less than
			*/

			//First we make sure we are comparing BibliographyCitations only
			var xBibliographyCitation = x as BibliographyCitation;
			var yBibliographyCitation = y as BibliographyCitation;

			if (xBibliographyCitation == null || yBibliographyCitation == null) return 0;
			var xReference = xBibliographyCitation.Reference;
			var yReference = yBibliographyCitation.Reference;
			if (xReference == null || yReference == null) return 0;


			var xBibliographyEntry = x.GetTextUnits().ToString();
			var yBibliographyEntry = y.GetTextUnits().ToString();
			if (string.IsNullOrEmpty(xBibliographyEntry) || string.IsNullOrEmpty(yBibliographyEntry)) return 0; 
			
			var xFirstLetter = xBibliographyEntry.Substring(0, 1);
			var yFirstLetter = yBibliographyEntry.Substring(0, 1);
			
			var xIsCyrillic = IsCyrillic(xFirstLetter);
			var yIsCyrillic = IsCyrillic(yFirstLetter);
			
			if ((xIsCyrillic && yIsCyrillic) || (!xIsCyrillic && !yIsCyrillic)) return xBibliographyEntry.CompareTo(yBibliographyEntry);
			else if (xIsCyrillic && !yIsCyrillic) return -1;
			else if (!xIsCyrillic && yIsCyrillic) return 1;
			else return 0;

			
		}
		
		
		public bool IsCyrillic(string input)
		{
			if (string.IsNullOrEmpty(input)) return false;
			
			if (Regex.IsMatch(input, @"\p{IsCyrillic}")) return true;
			return false;
		}
	}
}