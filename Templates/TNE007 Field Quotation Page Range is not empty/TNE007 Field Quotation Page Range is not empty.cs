//TNE007
//Version 1.0
//Field Quotation Page Range is not empty - Citation has no quoted pages

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

			return !BuiltInTemplateCondition.PlaceholderCitationEmptyQuotationPageRange.IsMet(template, citation);
		}
	}
}
