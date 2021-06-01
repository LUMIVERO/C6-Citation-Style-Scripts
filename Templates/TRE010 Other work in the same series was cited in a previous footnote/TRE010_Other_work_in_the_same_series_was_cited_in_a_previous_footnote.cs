//TRE010
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
			//Other work in the same series was cited in a previous footnote
			if (citation == null) return false;

			var currentFootnoteCitation = citation as FootnoteCitation;
			if (currentFootnoteCitation == null) return false;
			
			Reference currentReference = currentFootnoteCitation.Reference;
			SeriesTitle currentSeries = currentReference.SeriesTitle;
			if (currentSeries == null && currentReference.ParentReference != null)
			{
				currentSeries = currentReference.ParentReference.SeriesTitle;
			}
			if (currentSeries == null) return false;
			
			
			// regarding placeholderCitation.PersonOnly: see comment for RepeatingInTextCitation
			if (currentFootnoteCitation.RuleSetOverride != RuleSetOverride.None) return false;
			if (currentFootnoteCitation.YearOnly) return false;

			foreach (var otherFootnoteCitation in currentFootnoteCitation.CitationManager.FootnoteCitations)
			{
				if (otherFootnoteCitation == currentFootnoteCitation) break; //we just look in the direction of the beginning of the document
				
				Reference otherReference = otherFootnoteCitation.Reference;
				if (otherReference == null) continue;
				
				SeriesTitle otherSeries = otherReference.SeriesTitle;
				if (otherSeries == null && otherReference.ParentReference != null)
				{
					otherSeries = otherReference.ParentReference.SeriesTitle;
				}
				if (otherSeries == null) continue;				

				if (otherSeries == currentSeries && otherReference != currentReference)
				{
					return true;
				}
			}

			//still here? nothing found, return false
			return false;
		}
	}
}
