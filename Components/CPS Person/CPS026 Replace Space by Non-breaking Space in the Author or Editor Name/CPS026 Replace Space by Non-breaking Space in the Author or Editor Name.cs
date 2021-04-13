//C6#CPS026
//Description: Replace Space by Non-breaking Space in the Author or Editor Name
//Version: 1.0 

using System;
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

			PersonFieldElement personFieldElement = componentPart.GetFieldElements().OfType<PersonFieldElement>().FirstOrDefault();
			if (personFieldElement == null) return null;

			IEnumerable<Person> persons = personFieldElement.GetPersons(citation.Reference);
			if (persons == null || !persons.Any()) return null;

			bool found = false;
			TextUnitCollection textUnits = personFieldElement.GetTextUnits(citation, template);
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
				componentPart.Elements.ReplaceItem(personFieldElement, textUnits.TextUnitsToLiteralElements(componentPart));
			}
			
			return null;
		}
	}
}
