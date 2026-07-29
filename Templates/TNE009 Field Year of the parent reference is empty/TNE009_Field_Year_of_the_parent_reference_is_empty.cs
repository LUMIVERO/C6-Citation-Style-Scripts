//TNE009
//Description:	Field "Year" of the parent reference is empty
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
			if (citation.Reference == null) return false;
			if (citation.Reference.ParentReference == null) return false;

			return string.IsNullOrEmpty(citation.Reference.ParentReference.Year)
				&& string.IsNullOrEmpty(citation.Reference.ParentReference.YearResolved);

		}
	}
}
