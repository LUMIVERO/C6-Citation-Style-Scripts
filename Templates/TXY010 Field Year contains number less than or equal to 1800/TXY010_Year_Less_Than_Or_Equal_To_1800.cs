//TXY010

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

			if (string.IsNullOrEmpty(citation.Reference.YearResolved)) return false;

			DateTime result;
			if (!DateTimeInformation.TryParse(citation.Reference.YearResolved, out result)) return false;

			if (result.Year > 1800) return false;
			return true;
		}
	}
}