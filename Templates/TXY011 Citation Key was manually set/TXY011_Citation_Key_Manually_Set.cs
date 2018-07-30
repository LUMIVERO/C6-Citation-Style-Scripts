//TXY011

using System;
using System.Collections.Generic;
      
namespace SwissAcademic.Citavi.Citations
{
	public class CustomTemplateCondition : ITemplateConditionMacro
	{
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			
			return citation.Reference.CitationKeyUpdateType != 0;
			
		}
	}
}