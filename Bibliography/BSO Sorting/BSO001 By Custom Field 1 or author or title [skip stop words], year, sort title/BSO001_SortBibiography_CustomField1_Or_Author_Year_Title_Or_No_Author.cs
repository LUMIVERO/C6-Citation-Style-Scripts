//BSO001
//21.06.2018 
//Version 1.1

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
            //Version 1.1 Corrected compilation error "IReference" not found
			//First we make sure we are comparing BibliographyCitations only
			var xBibliographyCitation = x as BibliographyCitation;
			var yBibliographyCitation = y as BibliographyCitation;

			if (xBibliographyCitation == null || yBibliographyCitation == null) return 0;
			var xReference = xBibliographyCitation.Reference;
			var yReference = yBibliographyCitation.Reference;
			if (xReference == null || yReference == null) return 0;


			var sortDescriptors = new List<PropertySortDescriptor<Reference>>();
			sortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.AuthorsOrEditorsOrOrganizations, ListSortDirection.Ascending));
			var authorsComparer = new CitationComparer(sortDescriptors);

			var authorsCompareResult = authorsComparer.Compare(x, y);
			if (authorsCompareResult != 0) return authorsCompareResult;
			
			
			string xSortTitle = GetSortTitle(xReference);
			string ySortTitle = GetSortTitle(yReference);
			
			var sortTitleCompareResult = xSortTitle.CompareTo(ySortTitle);
			if (sortTitleCompareResult != 0) return sortTitleCompareResult;
			
			
			sortDescriptors.Clear();
			sortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.YearResolved, ListSortDirection.Descending));
			var yearComparer = new CitationComparer(sortDescriptors);
			
			return yearComparer.Compare(x, y);
			
		}
		
		
		private static string GetSortTitle (Reference reference)
		{
			//change CustomField1 to some other CustomFieldN where the sorting title can be found or
			//place two slashes in front of the following line to NOT make use of a special sorting title field at all
			//if (!string.IsNullOrEmpty(reference.CustomField1)) return reference.CustomField1;
			
			//still here? Then the sort title is derived from the title
			var particlesToIgnore = new string[] {
				"am",
				"an",
				"auf",
				"das",
				"der",
				"die",
				"ein",
				"eine",
				"im",
				"in",
				"inmitten",
				"nach",
				"über",
				"ueber",
				"vom",
				"von",
				"von",
				"was",
				"wie",
				"wo",
				"zu",
				"zum",
				"zur",
				"zwischen",

				"a",
				"the",
				
				"le",
				"la",
				"les"
			};
			
			var particlesRegEx = new Regex(@"\b(" + string.Join("|", particlesToIgnore) + @")\b", RegexOptions.IgnoreCase);
			var wordsRegex = new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*");

			string sortTitle = string.Empty;
			var titleWords = wordsRegex.Split(reference.Title.Trim());
			int firstNonParticleToIgnoreIndex = -1;

			foreach (string word in titleWords)
			{
				if (string.IsNullOrEmpty(word)) break;

				firstNonParticleToIgnoreIndex++;
				if (!particlesRegEx.IsMatch(word)) break;
			}

			if (firstNonParticleToIgnoreIndex == -1) return reference.Title; 	//either title is empty or consists of particles to ignore ONLY
			if (firstNonParticleToIgnoreIndex == 0) return reference.Title;		//title starts with a relevant word
			
			//"[" + string.Join(" ", titleWords, 0, firstNonParticleIndex) + "] " + string.Join(" ", titleWords, firstNonParticleIndex, titleWords.Length - firstNonParticleIndex);
			return string.Join(" ", titleWords, firstNonParticleToIgnoreIndex, titleWords.Length - firstNonParticleToIgnoreIndex);
		}
	}
}