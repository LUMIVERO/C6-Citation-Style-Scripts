//TXY012

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
		//Version 1.1 Considers series title of parent reference, if applicable
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			
			SeriesTitle seriesTitle = null;
			if (citation.Reference.HasCoreField(ReferenceTypeCoreFieldId.SeriesTitle))
			{
				seriesTitle = citation.Reference.SeriesTitle;
			}
			else
			{
				if (citation.Reference.ParentReference != null && citation.Reference.ParentReference.HasCoreField(ReferenceTypeCoreFieldId.SeriesTitle))
				{
					seriesTitle = citation.Reference.ParentReference.SeriesTitle;
				}
			}
			
			if (seriesTitle == null) return false;

			return string.Equals(seriesTitle.Name, "NAME", StringComparison.OrdinalIgnoreCase);
			//return seriesTitle.Name.StartsWith("NAME", StringComparison.OrdinalIgnoreCase);
			//return seriesTitle.Name.EndsWith("NAME", StringComparison.OrdinalIgnoreCase);
		}
	}
}
