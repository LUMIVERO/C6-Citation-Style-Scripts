//TXY022
//Description:	Field "Page range" of the reference (not the citation!) has certain numbering type
//Version 1.0

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

			bool activateOnPages = false;				//Seitennummern
			bool activateOnColumns = false;				//Spaltennummern			
			bool activateOnParagraphs = true;			//Paragraphen
			bool activateOnMargins = false;				//Randnummern			
			bool activateOnOthers = false;				//Andere

			if (citation == null) return false;
			if (citation.Reference == null) return false;
			if (citation.Reference.PageRange == null) return false;
			
			if 
			(
				(activateOnPages && citation.Reference.PageRange.NumberingType == NumberingType.Page) ||
				(activateOnColumns && citation.Reference.PageRange.NumberingType == NumberingType.Column) ||
				(activateOnParagraphs && citation.Reference.PageRange.NumberingType == NumberingType.Paragraph) ||
				(activateOnMargins && citation.Reference.PageRange.NumberingType == NumberingType.Margin) ||
				(activateOnOthers && citation.Reference.PageRange.NumberingType == NumberingType.Other)
			)
			{
				return true;
			}
			
			return false;

		}
	}
}
