//C6#COT038
//Description: Display o. S. if Quotation Page Range is empty
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
			handled = false;

			if (citation == null) return null;

			Reference currentReference = citation.Reference;
			if (currentReference == null) return null;

			if (componentPart == null) return null;
			if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;

			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return null;
			if (placeholderCitation.Entry == null) return null;

			PageRangeFieldElement pageRangeFieldElement = componentPart.Elements.OfType<PageRangeFieldElement>().FirstOrDefault();
			if (pageRangeFieldElement == null) return null;


			var outputString = "o. S.";
			if (placeholderCitation.PageRange != null || !string.IsNullOrWhiteSpace(placeholderCitation.PageRange.OriginalString)) return null;

			var outputLiteralElement = new LiteralElement(componentPart, outputString);
			outputLiteralElement.FontStyle = pageRangeFieldElement.PageMultiStartFontStyle;
			componentPart.Elements.ReplaceItem(pageRangeFieldElement, outputLiteralElement);


			//all literal elements should always be output:
			foreach (LiteralElement literalElement in componentPart.Elements.OfType<LiteralElement>())
			{
				literalElement.ApplyCondition = ElementApplyCondition.Always;
			}

			return null;
		}
	}
}
