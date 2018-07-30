//BSO002
//21.06.2018 
//Version 1.0

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
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
			
			
			//Set the NoBib flag to true for PersonalCommunication (aka "Persönliche Kommunikation")
			//if (xReference.ReferenceType == ReferenceType.PersonalCommunication) xBibliographyCitation.NoBib = true;
			//if (yReference.ReferenceType == ReferenceType.PersonalCommunication) yBibliographyCitation.NoBib = true;
			
			//now the sorting can begin
			var defaultCitationComparer = CitationComparer.AuthorYearTitleOrNoAuthorThenTitleYearAscending;
			var defaultResult = defaultCitationComparer.Compare(x, y);
			
			if (defaultResult != 0) return defaultResult;
			
			
			var yearTitleSortDescriptors = new List<PropertySortDescriptor<Reference>>();
			yearTitleSortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.YearResolved));
			yearTitleSortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.Title));
			yearTitleSortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.Volume));
			yearTitleSortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.Number));
			var yearTitleComparer = new CitationComparer(yearTitleSortDescriptors);
			
			return yearTitleComparer.Compare(x, y);
		}
	}
}

