//#43215
//Version 1.1
//Feld CustomField1 des übergeordneten Titels ist nicht leer

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
			
			var CustomField1 = citation.Reference.ParentReference.CustomField1;
			
			return !string.IsNullOrEmpty(CustomField1);
			
		}
	}
}
