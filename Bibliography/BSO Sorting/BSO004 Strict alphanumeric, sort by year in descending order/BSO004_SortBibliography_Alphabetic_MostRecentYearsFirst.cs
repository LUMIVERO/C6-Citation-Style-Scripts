// #C5_43322
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
			//First we make sure we are comparing BibliographyCitations only
			var xBibliographyCitation = x as BibliographyCitation;
			var yBibliographyCitation = y as BibliographyCitation;

			if (xBibliographyCitation == null || yBibliographyCitation == null) return 0;
			var xReference = xBibliographyCitation.Reference;
			var yReference = yBibliographyCitation.Reference;
			if (xReference == null || yReference == null) return 0;

			
			var xYearResolved = xReference.YearResolved;
			var yYearResolved = yReference.YearResolved;
			
			var xBibliographyEntry = x.GetTextUnits().ToString();
			var yBibliographyEntry = y.GetTextUnits().ToString();
			
			string xBibliographyEntryBeforeYear = xBibliographyEntry;
			string yBibliographyEntryBeforeYear = yBibliographyEntry;
			
			//Autor, Herausgeber oder Institution oder was auch immer im Template zu Beginn vor der Jahresangabe steht, aufsteigend
			if (!string.IsNullOrEmpty(xYearResolved))
			{
				int xIndexOfYearResolved = xBibliographyEntry.IndexOf(xYearResolved, 0);
				if (xIndexOfYearResolved > 0) xBibliographyEntryBeforeYear = xBibliographyEntry.Substring(0, xIndexOfYearResolved);
			}
			if (!string.IsNullOrEmpty(yYearResolved))
			{
				int yIndexOfYearResolved = yBibliographyEntry.IndexOf(yYearResolved, 0);
				if (yIndexOfYearResolved > 0) yBibliographyEntryBeforeYear = yBibliographyEntry.Substring(0, yIndexOfYearResolved);
			}
			var alphabeticalSortResult = xBibliographyEntryBeforeYear.CompareTo(yBibliographyEntryBeforeYear);
			if (alphabeticalSortResult != 0) return alphabeticalSortResult;
			
			//Jahr ermittelt, absteigend
			var sortDescriptors = new List<PropertySortDescriptor<Reference>>();
			sortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.YearResolved, ListSortDirection.Descending));
			var yearComparer = new CitationComparer(sortDescriptors);
			var yearSortResult = yearComparer.Compare(x, y);
			if (yearSortResult != 0) return yearSortResult;
			
			//Titel, aufsteigend
			sortDescriptors.Clear();
			sortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.Title, ListSortDirection.Ascending));
			var titleComparer = new CitationComparer(sortDescriptors);
			var titleSortResult = titleComparer.Compare(x, y);
			if (titleSortResult != 0) return titleSortResult;
			
			//Band, absteigend
			sortDescriptors.Clear();
			sortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.Volume, ListSortDirection.Descending));
			var volumeComparer = new CitationComparer(sortDescriptors);
			var volumeSortResult = volumeComparer.Compare(x, y);
			if (volumeSortResult != 0) return volumeSortResult;
			
			//Datum des Zugriffs, absteigend
			sortDescriptors.Clear();
			sortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.AccessDate, ListSortDirection.Descending));
			var accessDateComparer = new CitationComparer(sortDescriptors);
			var accessDateSortResult = accessDateComparer.Compare(x, y);
			return accessDateSortResult;
			
		}
	}
}