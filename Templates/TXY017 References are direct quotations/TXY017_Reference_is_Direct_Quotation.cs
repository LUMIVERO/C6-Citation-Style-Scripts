//TXY017
//2018-06-24
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
		//Reference is a direct quotation
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			
			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return false;
			
			if (placeholderCitation.Entry == null) return false;

			return placeholderCitation.Entry.QuotationType == QuotationType.DirectQuotation;
		}
	}
}