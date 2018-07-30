//TNE005

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

			var reference = citation.Reference;
			if (reference == null) return false;

			var placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return false;

			return placeholderCitation.PageRange == null || string.IsNullOrWhiteSpace(placeholderCitation.PageRange.OriginalString);
		}
	}
}