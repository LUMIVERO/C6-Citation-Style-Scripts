//TRE006
//Reference has been cited only once

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
			//Titel ist nur einmal in der FN zitiert
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			
			FootnoteCitation footnoteCitation = citation as FootnoteCitation;
			if (footnoteCitation == null) return false;
			
			return footnoteCitation.IsUniqueFootnote;
			
		}
	}
}
