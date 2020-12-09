//TRE014
//Version 1.0
//Previous citation of the reference is NOT part of a caption

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
			if (citation == null) return true;
			if (citation.Reference == null) return true;
			
			PlaceholderCitation thisCitation = citation as PlaceholderCitation;
			if (thisCitation == null) return true; //only PlaceholderCitations can be part of a caption and can have a predecessor-PlaceholderCitation that can be part of a caption
			
			//caution: the property thisCitation.PreviousPlaceholderCitation may give wrong results and should currently (C6.7) not be used 
			PlaceholderCitation previousCitation = thisCitation.PreviousCitation as PlaceholderCitation; 
			if (previousCitation == null) return true; //ditto
			
			if (previousCitation.IsPartOfCaption) return false;
			
			return true;
		}
	}
}
