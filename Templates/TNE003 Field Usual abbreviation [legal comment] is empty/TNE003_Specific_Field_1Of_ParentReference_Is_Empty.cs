//TNE003
//Field "SpecificField1" of Reference OR Parent Reference is empty

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
		
		if (citation.Reference.ReferenceType == ReferenceType.LegalCommentary)
		{
			return string.IsNullOrWhiteSpace(citation.Reference.SpecificField1);
		}
		
		if (citation.Reference.ReferenceType == ReferenceType.ContributionInLegalCommentary)
		{
			if (citation.Reference.ParentReference == null) return false;
			return string.IsNullOrWhiteSpace(citation.Reference.ParentReference.SpecificField1); 
		}

		return false;
		}
	}
}