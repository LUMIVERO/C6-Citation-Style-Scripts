//C6#COT031
//Description: Replace Divis by Non-breaking Hyphen in Case Number (Aktenzeichen)
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
			if (citation.Reference == null) return null;
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;


			TextFieldElement textFieldElement = componentPart.Elements.OfType<TextFieldElement>().FirstOrDefault();
			//SwissAcademic.Citavi.Citations.TextFieldElement
			if (textFieldElement == null) return null;

			bool found = false;
			TextUnitCollection textUnits = textFieldElement.GetTextUnits(citation, template);
			if (textUnits == null) return null;
			
			foreach(ITextUnit textUnit in textUnits)
			{
				if (textUnit.Text.Contains("-"))
				{
					found = true;
					textUnit.Text = textUnit.Text.Replace("-", "\u2011");
				}
			}
			
			if (found)
			{
				componentPart.Elements.ReplaceItem(textFieldElement, textUnits.TextUnitsToLiteralElements(componentPart));
			}
			
			return null;
		}
	}
}
