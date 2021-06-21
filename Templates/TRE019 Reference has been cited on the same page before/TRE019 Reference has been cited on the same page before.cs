//TRE019
//Version 1.0
//Reference has been cited on the same page before

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
			
			PlaceholderCitation currentPlaceholderCitation = citation as PlaceholderCitation;
			if (currentPlaceholderCitation == null) return false;
			
			PlaceholderCitation previousPlaceholderCitation = currentPlaceholderCitation.PreviousPlaceholderCitation;
			while (previousPlaceholderCitation != null)
			{
				if (previousPlaceholderCitation.PageInPublication != currentPlaceholderCitation.PageInPublication) return false;
				if (previousPlaceholderCitation.Reference != null && previousPlaceholderCitation.Reference.Equals(currentPlaceholderCitation.Reference))
				{
					return true;
				}
				
				previousPlaceholderCitation = previousPlaceholderCitation.PreviousPlaceholderCitation;
			}
			
			//still here, then we had no luck iterating on same page & looking for same reference
			return false;
		}
	}
}
