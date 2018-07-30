//TRE001

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
			//Another contribution from the same parent reference has already been cited in the same footnote.	
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			if (citation.Reference.ParentReference == null) return false;
			
			var thisFootnoteCitation = citation as FootnoteCitation;
			if (thisFootnoteCitation == null) return false;
			
			var previousFootnoteCitation = thisFootnoteCitation.PreviousFootnoteCitation;
			if (previousFootnoteCitation == null) return false;
			
			if (previousFootnoteCitation.FootnoteIndex != thisFootnoteCitation.FootnoteIndex) return false;
			if (previousFootnoteCitation.Reference == null) return false;
			if (previousFootnoteCitation.Reference == thisFootnoteCitation.Reference) return false;
			if (previousFootnoteCitation.Reference.ParentReference == null) return false;
			if (previousFootnoteCitation.Reference.ParentReference != thisFootnoteCitation.Reference.ParentReference) return false;

			
			//all conditions met
			return true;
		}
	}
}