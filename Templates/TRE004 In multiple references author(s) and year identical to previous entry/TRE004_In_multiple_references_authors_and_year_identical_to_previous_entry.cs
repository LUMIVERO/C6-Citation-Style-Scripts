//TRE004
//In multiple references author(s) and year identical to previous entry 

using System;
using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;

namespace SwissAcademic.Citavi.Citations
{
	public class CustomTemplateCondition
		:
		ITemplateConditionMacro
	{
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;
			if (citation.Reference == null) return false;

			//citation must be part of multiple citation
			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return false;
			if (!placeholderCitation.IsPartOfMultipleCitation) return false;

			//citation must not be the first citation in a multiple citation and must have a predecessor
			if (placeholderCitation.IsFirstInMultipleCitation) return false;
			PlaceholderCitation previousPlaceholderCitation = placeholderCitation.PreviousPlaceholderCitation;
			if (previousPlaceholderCitation == null) return false;


			Reference thisReference = placeholderCitation.Reference;
			Reference previousReference = previousPlaceholderCitation.Reference;
			if (previousReference == null) return false;

			//reference must have the same year as its predecessor
			string thisYear = thisReference.YearResolved;
			string previousYear = previousReference.YearResolved;
			if (!thisYear.Equals(previousYear)) return false;

			//reference must have the same authors as its predecessor
			IEnumerable<Person> thisAuthors = thisReference.AuthorsOrEditorsOrOrganizations.ToList();
			IEnumerable<Person> previousAuthors = previousReference.AuthorsOrEditorsOrOrganizations.ToList();
			if (!thisAuthors.SequenceEqual(previousAuthors)) return false;

			return true;
		}
	}
}
