//C6#COT029
//Description: Suppress output of component part depending on page range numbering type
//Version: 1.0  

using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;

namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
		:
		IComponentPartFilter
	{
		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{

			bool doSuppressOnPages = false;					//Seitennummern
			bool doSuppressOnColumns = false;				//Spaltennummern			
			bool doSuppressOnParagraphs = false;			//Paragraphen
			bool doSuppressOnMargins = true;				//Randnummern			
			bool doSuppressOnOthers = false;				//Andere			

			//do not edit
			handled = false;
			if (citation == null) return null;
			if (componentPart == null) return null;
			if (componentPart.Elements == null) return null;
			if (!componentPart.Elements.Any()) return null;
			
			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return null;
			if (placeholderCitation.Entry == null) return null;
			
			if 
			(
				(doSuppressOnPages && citation.Reference.PageRange.NumberingType == NumberingType.Page) ||
				(doSuppressOnColumns && citation.Reference.PageRange.NumberingType == NumberingType.Column) ||
				(doSuppressOnParagraphs && citation.Reference.PageRange.NumberingType == NumberingType.Paragraph) ||
				(doSuppressOnMargins && citation.Reference.PageRange.NumberingType == NumberingType.Margin) ||
				(doSuppressOnOthers && citation.Reference.PageRange.NumberingType == NumberingType.Other)
			)
			{
				handled = true;
				return null;
			}
			
			return null;
		}
	}
}
