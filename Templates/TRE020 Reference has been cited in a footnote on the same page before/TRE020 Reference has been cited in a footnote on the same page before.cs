//TRE020
//Version 1.0
//Reference has been cited in a footnote on the same page before

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
			
			FootnoteCitation currentFootnoteCitation = citation as FootnoteCitation;
			if (currentFootnoteCitation == null) return false;
			
			FootnoteCitation previousFootnoteCitation = currentFootnoteCitation.PreviousFootnoteCitation;
			while (previousFootnoteCitation != null)
			{
				if (previousFootnoteCitation.PageInPublication != currentFootnoteCitation.PageInPublication) return false;
				if (previousFootnoteCitation.Reference != null && previousFootnoteCitation.Reference.Equals(currentFootnoteCitation.Reference))
				{
					return true;
				}
				
				previousFootnoteCitation = previousFootnoteCitation.PreviousFootnoteCitation;
			}
			
			//still here, then we had no luck iterating on same page & looking for same reference
			return false;
		}
	}
}
