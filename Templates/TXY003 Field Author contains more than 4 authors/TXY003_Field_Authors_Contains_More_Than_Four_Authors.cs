//TXY003

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
			//More than 4 authors, editors, organizations
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			
			var authors = citation.Reference.AuthorsOrEditorsOrOrganizations;
			if (authors == null || authors.Count == 0) return false;
			
			
			return authors.Count() > 4;
		}
	}
}