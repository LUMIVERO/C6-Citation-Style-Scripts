//TRE017
//Version 1.0
//Reference has been cited exactly once before

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
		//Reference has been cited exactly once before
		
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			
			PlaceholderCitation currentPlaceholderCitation = citation as PlaceholderCitation;
			if (currentPlaceholderCitation == null) return false;
			
			CitationManager citationManager = citation.CitationManager;
			if (citationManager == null) return false;
			if (citationManager.PlaceholderCitations == null || citationManager.PlaceholderCitations.Count <= 1) return false; //no 2nd mention possible	
			if (currentPlaceholderCitation.IsUniquePlaceholderCitation) return false; 		//there is just one
			if (!currentPlaceholderCitation.IsRepeatingCitation) return false; 	//it has not been mentioned before, therefore in cannot be the 2nd (or higher) mention
			
			//still here? we iterate over all footnote citations
			int previousMentions = 0;
			foreach(PlaceholderCitation otherPlaceholderCitation in citationManager.PlaceholderCitations)
			{
				if (otherPlaceholderCitation == null) continue;
				if (otherPlaceholderCitation.Reference == null) continue;
				if (otherPlaceholderCitation.Equals(currentPlaceholderCitation)) break; //we reached the current footnote citation
				
				if (otherPlaceholderCitation.Reference.Equals(currentPlaceholderCitation.Reference))
				{
					previousMentions++; //found another previous mention
				}
			}
			
			return previousMentions == 1;
		}
	}
}
