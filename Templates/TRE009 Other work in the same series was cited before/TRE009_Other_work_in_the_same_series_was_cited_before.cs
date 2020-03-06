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
			//Other work in the same series was cited before
            if (citation == null) return false;

            var currentPlaceholderCitation = citation as PlaceholderCitation;
            if (currentPlaceholderCitation == null) return false;
            
			Reference currentReference = currentPlaceholderCitation.Reference;
			SeriesTitle currentSeries = currentReference.SeriesTitle;
			if (currentSeries == null && currentReference.ParentReference != null)
			{
				currentSeries = currentReference.ParentReference.SeriesTitle;
			}
			if (currentSeries == null) return false;
			
			
            // regarding placeholderCitation.PersonOnly: see comment for RepeatingInTextCitation
            if (currentPlaceholderCitation.RuleSetOverride != RuleSetOverride.None) return false;
            if (currentPlaceholderCitation.YearOnly) return false;


			CitationManager citationManager = currentPlaceholderCitation.CitationManager;
			if (citationManager == null) return false;
			
			// we iterate over placeholder citations inside the same ruleset ONLY, i.e. in-text citations OR footnote citations
			IEnumerable<PlaceholderCitation> otherPlaceholderCitations = null;
			if (currentPlaceholderCitation.CitationType == CitationType.Footnote)
			{
				otherPlaceholderCitations = citationManager.FootnoteCitations.Select(item => item as PlaceholderCitation);
			}
			else if (currentPlaceholderCitation.CitationType == CitationType.InText)
			{
				otherPlaceholderCitations = citationManager.InTextCitations.Select(item => item as PlaceholderCitation);
			}


            foreach (PlaceholderCitation otherPlaceholderCitation in otherPlaceholderCitations)
            {
                if (otherPlaceholderCitation == currentPlaceholderCitation) break; //we just look in the direction of the beginning of the document
				
                Reference otherReference = otherPlaceholderCitation.Reference;
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
