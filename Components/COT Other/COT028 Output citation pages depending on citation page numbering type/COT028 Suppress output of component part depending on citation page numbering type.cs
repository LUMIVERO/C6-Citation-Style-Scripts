//C6#COT028
//Description: Suppress output of component part depending on page numbering type
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
				(doSuppressOnPages && placeholderCitation.Entry.PageRange.NumberingType == NumberingType.Page) ||
				(doSuppressOnColumns && placeholderCitation.Entry.PageRange.NumberingType == NumberingType.Column) ||
				(doSuppressOnParagraphs && placeholderCitation.Entry.PageRange.NumberingType == NumberingType.Paragraph) ||
				(doSuppressOnMargins && placeholderCitation.Entry.PageRange.NumberingType == NumberingType.Margin) ||
				(doSuppressOnOthers && placeholderCitation.Entry.PageRange.NumberingType == NumberingType.Other)
			)
			{
				handled = true;
				return null;
			}
			
			return null;
		}
	}
}
