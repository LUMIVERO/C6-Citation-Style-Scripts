//C6#COT012
//C5#431621
//Description: Shorten page number range
//Version: 2.2
//Behandlung von Seitenzahlbereichen mit genau 3 (oder ggf. 3 oder 4) Seiten: z.B.: 1-3 (oder ggf. 1-4) -> S. 1 ff.
//Genau 1 Seite, z.B.: 					1 				-> S. 1
//Genau 2 Seiten, z.B.:					1-2 			-> S. 1 f.
//Genau 3 (oder ggf. 4) Seiten, z.B.: 	1-3 (ggf. 1-4) 	-> S. 1 ff.  (DIESER FALL WIRD HIER BEHANDELT)
//4 bzw. 5 und mehr Seiten, z.B.:		1-5 			-> S. 1-5
//Version 2.2: Arrangement of numbering types (page, column, paragraph, margin, other) corrected according to GUI
//Version 2.1: 	Customizable, whether the script should apply to a page range with exactly 3 pages OR with 3 or 4 pages.
//Version 2: 	Uses PageNumber.IsFullyNumeric instead of PageNumber.IsNumeric. 
//Pls. deploy script version 1.1 instead, if you get compilation errors with your Citavi 5 version.

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
   			string suffixForRangeOf3or4 = "\u00A0ff.";		//inkl. geschütztem Leerzeichen (ggf. durch normales Leerzeichen ersetzen)
			bool rangeToTreatIsExactly3 = true;				//wenn false, dann soll ein Bereich von 3 ODER 4 Seiten mit " ff." verkürzt werden (sonst nur bei genau 3 Seiten)
			
			handled = false;

			if (citation == null) return null;
			if (citation.Reference == null) return null;
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;

			FieldElement fieldElement;

			PageRangeFieldElement pageRangeFieldElement = componentPart.Elements.OfType<PageRangeFieldElement>().FirstOrDefault();
			if (pageRangeFieldElement == null) return null;

			PageRange pageRange = null;

			if (pageRangeFieldElement is QuotationPageRangeFieldElement)
			{
				PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
				if (placeholderCitation == null) return null;
				if (placeholderCitation.Entry == null) return null;

				pageRange = placeholderCitation.Entry.PageRange;

			}
			else if (pageRangeFieldElement is PageRangeFieldElement)
			{
				pageRange = citation.Reference.PageRange;
			}
			else
			{
				//what should that be ?
				return null;
			}

			if (pageRange == null) return null;

			if (pageRange.StartPage == null) return null;
			if (!pageRange.StartPage.IsFullyNumeric) return null;

			int startPage = pageRange.StartPage.Number ?? 0;
			if (startPage == 0) return null;

			if (pageRange.EndPage == null) return null;
			if (!pageRange.EndPage.IsFullyNumeric) return null;

			int endPage = pageRange.EndPage.Number ?? 0;
			if (endPage == 0) return null;

			int delta = 0;

			if (endPage > startPage) delta = endPage - startPage;
			else delta = 0;

			//we only treat the special case delta = 2 (optional 2 or 3) (three pages, optional three or four pages)
			//delta = 0 (over one page) is handled directly from the elements settings
			//delta = 1 (over two pages) ditto
			//delta > 2 or 3 (over 4 or 5 or more pages) ditto

			if ( (rangeToTreatIsExactly3 == true && delta == 2) || (rangeToTreatIsExactly3 == false && (delta == 2 || delta == 3)) )
			{
				switch(pageRange.NumberingType)
				{
					case NumberingType.Page:
					default:
					{
						pageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
						pageRangeFieldElement.PageMultiSuffix.Text = suffixForRangeOf3or4;
					}
					break;
					
					case NumberingType.Column:
					{	
						pageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
						pageRangeFieldElement.ColumnMultiSuffix.Text = suffixForRangeOf3or4;
					}
					break;
					
					case NumberingType.Paragraph:
					{
						pageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.StartPageOnly;
						pageRangeFieldElement.ParagraphMultiSuffix.Text = suffixForRangeOf3or4;
					}
					break;
					
					case NumberingType.Margin:
					{
						pageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
						pageRangeFieldElement.MarginMultiSuffix.Text = suffixForRangeOf3or4;
					}
					break;
					
					case NumberingType.Other:
					{
						pageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.StartPageOnly;
						pageRangeFieldElement.OtherMultiSuffix.Text = suffixForRangeOf3or4;
					}
					break;
				}
			}

			return null; //handled remains false, because Citavi will do the formatting with the changed field element's properties

		}
	}
}
