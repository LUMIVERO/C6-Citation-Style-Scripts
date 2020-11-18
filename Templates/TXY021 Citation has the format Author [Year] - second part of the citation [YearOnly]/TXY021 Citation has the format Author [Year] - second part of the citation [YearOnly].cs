//TXY021 Citation has the format "Author (Year)" - this template covers the second part of the citation (YearOnly)
//Version 1.0

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
			
			
			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return false;
			if (placeholderCitation.NoPar) return false;
			if (placeholderCitation.PersonOnly) return false;
			if (!placeholderCitation.YearOnly) return false;
			
			//all conditions met
			return true;
		}
	}
}
