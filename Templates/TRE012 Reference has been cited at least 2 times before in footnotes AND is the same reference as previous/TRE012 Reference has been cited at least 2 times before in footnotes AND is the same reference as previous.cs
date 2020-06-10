//TRE012
//Version 1.1
//Reference has been cited at least 2 times before in footnotes AND is the same reference as previous

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
		//Die unmittelbar vorhergehende Fußnote ist von demselben Titel. Außerdem ist der Titel der unmittelbar vorhergehenden Fußnote bereits weiter oben schon einmal zitiert worden. Daher handelt es sich bei der aktuellen Fußnote um mindestens die dritte Erwähnung dieses Titels.
		
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			
			var thisFootnoteCitation = citation as FootnoteCitation;
			if (thisFootnoteCitation == null) return false;
			
			var previousFootnoteCitation = thisFootnoteCitation.PreviousFootnoteCitation;
			if (previousFootnoteCitation == null) return false;
				
			if (thisFootnoteCitation.IsUniqueFootnote) return false;
			if (!thisFootnoteCitation.IsRepeatingFootnote) return false;
	
			if (previousFootnoteCitation.Reference != thisFootnoteCitation.Reference) return false;		
			if (!previousFootnoteCitation.IsRepeatingFootnote) return false;
			
			return true;
		}
	}
}
