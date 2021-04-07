//TNE008
//Description:	Field "Author" of the parent reference is empty
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

			if (!citation.Reference.ParentReference.ReferenceType.CoreFields.Contains(ReferenceTypeCoreFieldId.Authors)) return false;

            return citation.Reference.ParentReference.Authors.Count == 0;

		}
	}
}
