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
		//Consecutive citation (same series)
		//This applies to in-text and footnote citations only, not bibliography citations. 
		//Contact Citavi support if you require a condition for bibliography citations.
		
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;

			//Make sure we are dealing with PlaceholderCitations (i.e. in-text and footnote citations, not bibliography entries)	
			var currentPlaceholderCitation = citation as PlaceholderCitation;
            if (currentPlaceholderCitation == null) return false;

            var previousPlaceholderCitation = currentPlaceholderCitation.PreviousPlaceholderCitation;
            if (previousPlaceholderCitation == null) return false;
			
			
			//determine the series of the current reference, if necessary by looking at its parent reference
            var currentReference = citation.Reference;
            if (currentReference == null) return false;
			
			var currentSeries = currentReference.SeriesTitle;
			if (currentSeries == null && currentReference.ParentReference != null) currentSeries = currentReference.ParentReference.SeriesTitle;
			if (currentSeries == null) return false;

			//determine the series of the previous reference, if necessary by looking at its parent reference
			//stay within the same rule set, i.e. footnote or in-text
			Reference previousReference = null;
			if (currentPlaceholderCitation.CitationType == CitationType.Footnote)
			{
				var currentFootnoteCitation = currentPlaceholderCitation as FootnoteCitation;
				if (currentFootnoteCitation == null) return false;
				
				var previousFootnoteCitation = currentFootnoteCitation.PreviousFootnoteCitation;
				if (previousFootnoteCitation == null) return false;
				previousReference = previousFootnoteCitation.Reference;
			}
			else if (currentPlaceholderCitation.CitationType == CitationType.InText)
			{
				var currentInTextCitation = currentPlaceholderCitation as InTextCitation;
				if (currentInTextCitation == null) return false;
				
				var previousInTextCitation = currentInTextCitation.PreviousInTextCitation;
				if (previousInTextCitation == null) return false;
				previousReference = previousInTextCitation.Reference;
			}
            if (previousReference == null) return false;
			
			var previousSeries = previousReference.SeriesTitle;
			if (previousSeries == null && previousReference.ParentReference != null) previousSeries = previousReference.ParentReference.SeriesTitle;
			if (previousSeries == null) return false;
			

            bool conditionMet = false;

            conditionMet = currentSeries == previousSeries;
            if (!conditionMet) return false;

            //https://github.com/Citavi/Citavi/issues/744
            bool noRuleSetOverride = currentPlaceholderCitation.RuleSetOverride == RuleSetOverride.None ||
                (currentPlaceholderCitation.CitationType == CitationType.Footnote && currentPlaceholderCitation.RuleSetOverride == RuleSetOverride.Footcit) ||
                (currentPlaceholderCitation.CitationType == CitationType.InText && currentPlaceholderCitation.RuleSetOverride == RuleSetOverride.Textcit);

            // currentPlaceholderCitation.PersonOnly: siehe Kommentar bei RepeatingInTextCitation
            conditionMet =
                noRuleSetOverride &&
                currentPlaceholderCitation.YearOnly == false &&
                currentPlaceholderCitation.Entry != null;

            if (!conditionMet) return false;

            return true;
		}
	}
}
