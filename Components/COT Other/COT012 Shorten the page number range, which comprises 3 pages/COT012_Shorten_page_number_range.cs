//C6#CPS019
//C5#431621
//Description: Shorten page number range
//Version: 2.0
//Behandlung von Seitenzahlbereichen mit genau 3 Seiten: z.B.: 17-19 -> S. 17 ff.
//Genau 1 Seite, z.B.: 17 -> S. 17
//Genau 2 Seiten, z.B. 17-18 -> S. 17 f.
//Genau 3 Seiten, z.B. 17-19 -> S. 17 ff.  (DIESER FALL WIRD HIER BEHANDELT)
//4 und mehr Seiten, z.B. 17-20 -> S. 17-20
//Version 2: Uses PageNumber.IsFullyNumeric instead of PageNumber.IsNumeric. 
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
   			string suffixForRangeOf3 = " ff.";
			
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

			//we only treat the special case delta = 2 (over three pages)
			//delta = 0 (over one page) is handled directly from the elements settings
			//delta = 1 (over two pages) ditto
			//delta > 2 (over 4 or more pages) ditto

			if (delta == 2)
			{
				switch(pageRange.NumberingType)
				{
					case NumberingType.Column:
					{	
						pageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
						pageRangeFieldElement.ColumnMultiSuffix.Text = suffixForRangeOf3;
					}
					break;
												
					case NumberingType.Margin:
					{
						pageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
						pageRangeFieldElement.MarginMultiSuffix.Text = suffixForRangeOf3;
					}
					break;
												
					case NumberingType.Other:
					{
						pageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.StartPageOnly;
						pageRangeFieldElement.OtherMultiSuffix.Text = suffixForRangeOf3;
					}
					break;
						
					case NumberingType.Page:
					default:
					{
						pageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
						pageRangeFieldElement.PageMultiSuffix.Text = suffixForRangeOf3;
					}
					break;
						
					case NumberingType.Paragraph:
					{
						pageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.StartPageOnly;
						pageRangeFieldElement.ParagraphMultiSuffix.Text = suffixForRangeOf3;
					}
					break;

				}

				

				
			}
			return null; //handled remains false, because Citavi will do the formatting with the changed field element's properties

		}
	}
}
