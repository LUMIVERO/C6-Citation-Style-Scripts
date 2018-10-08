// #C5_431622
// Version 1.0

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

			var personFieldElementCount = componentPart.Elements.Where(fieldElement => fieldElement is PersonFieldElement).Count();
			if (personFieldElementCount != 1) return null;

			var personFieldElement = componentPart.GetFieldElements().FirstOrDefault(fieldElement => fieldElement is PersonFieldElement) as PersonFieldElement;
			if (personFieldElement == null) return null;

			if (!personFieldElement.Abbreviate) return null;
			var abbreviation = personFieldElement.Abbreviation;
			if (abbreviation == null) return null;
			if (string.IsNullOrWhiteSpace(abbreviation.Text)) return null;

			if (abbreviation.Text != ", et al.") return null;


			var output = componentPart.GetTextUnitsUnfiltered(citation, template);
			var index = 0;
			foreach (ITextUnit textUnit in output)
			{
				if (textUnit.Text == ", et al.")
				{
					var newTextUnit = new LiteralTextUnit("et al.");
					newTextUnit.FontStyle = textUnit.FontStyle;

					textUnit.Text = ", ";
					textUnit.FontStyle = FontStyle.Neutral;
					output.Insert(++index, newTextUnit);

					break;
				}
				index++;
			}

			handled = true;
			return output;
		}
	}
}