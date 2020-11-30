//C6#COT037
//Description: Replace (double) quotation marks by single quotation marks in the Title field and other text field elements
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
				if (textUnit.Text.Contains("“"))						//EN, öffnend
				{
					found = true;
					textUnit.Text = textUnit.Text.Replace("“", "‘");
				}
				if (textUnit.Text.Contains("”"))						//EN, schließend
				{
					found = true;
					textUnit.Text = textUnit.Text.Replace("”", "’");
				}
				if (textUnit.Text.Contains("„"))						//DE, öffnend
				{
					found = true;
					textUnit.Text = textUnit.Text.Replace("„", "‚");
				}
				if (textUnit.Text.Contains("“"))						//DE, schließend
				{
					found = true;
					textUnit.Text = textUnit.Text.Replace("“", "‘");
				}
				if (textUnit.Text.Contains("»"))						//FR/CH, öffnend
				{
					found = true;
					textUnit.Text = textUnit.Text.Replace("»", "›");
				}
				if (textUnit.Text.Contains("«"))						//FR/CH, schließend
				{
					found = true;
					textUnit.Text = textUnit.Text.Replace("«", "‹");
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
