//TRE021
//Version 1.0
//Reference is cited in the first footnote of a page disregarding citationless footnotes

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
			if (previousFootnoteCitation == null) 
			{
				//there is no previous citation in a footnote, so this is the first one (disregarding reference-less footnotes)
				return true;
			}
						
			while (previousFootnoteCitation != null)
			{
				if (previousFootnoteCitation.FootnoteIndex != currentFootnoteCitation.FootnoteIndex)
				{
					if (previousFootnoteCitation.PageInPublication == currentFootnoteCitation.PageInPublication)
					{
						//a different previous footnote on the same page > this citation cannot be in the first footnote on the page
						return false;
					}
					else
					{
						//a different previous footnote on other page > this citation is in first footnote on page (disregarding reference-less footnotes)
						return true;
					}
				}
				else
				{
					if (previousFootnoteCitation.PageInPublication == currentFootnoteCitation.PageInPublication)
					{
						//the previous citation is in the same footnote on the same page; we cannot say anything yet
					}
					else
					{
						//the previous citation is in the same footnote, but on a different page: can we really detect page breaks INSIDE footnotes?
						return true;
					}
				}
				
				previousFootnoteCitation = previousFootnoteCitation.PreviousFootnoteCitation;
			}
			
			//still here? false!
            return false;
		}
	}
}
