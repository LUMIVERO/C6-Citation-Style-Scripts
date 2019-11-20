//C6#CPE005
//Description: Replace Space by Non-breaking Space in the Journal Name
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
			
			if (componentPart == null) return null;
			if (citation == null || citation.Reference == null) return null;

			Periodical periodical = citation.Reference.Periodical;
			if (periodical == null) return null;	

			PeriodicalFieldElement periodicalFieldElement = componentPart.Elements.OfType<PeriodicalFieldElement>().FirstOrDefault() as PeriodicalFieldElement;
			if (periodicalFieldElement == null) return null;

			bool found = false;
			TextUnitCollection textUnits = periodicalFieldElement.GetTextUnits(citation, template);
			if (textUnits == null) return null;

			foreach(ITextUnit textUnit in textUnits)
			{
				if (textUnit.Text.Contains(" "))
				{
					found = true;
					textUnit.Text = textUnit.Text.Replace(" ", "\u00A0");
				}
			}
			
			if (found)
			{
				componentPart.Elements.ReplaceItem(periodicalFieldElement, textUnits.TextUnitsToLiteralElements(componentPart));
			}
			
			return null;
		}
	}
}
