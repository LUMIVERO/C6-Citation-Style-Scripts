//TXY019 Field "Groups" of Parent Reference contains specific name
//Version 1.0

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
			if (citation.Reference.ParentReference == null) return false;

			return (citation.Reference.ParentReference.Groups.Any(item => item.Name.Equals("Group Name")));

		}
	}
}
