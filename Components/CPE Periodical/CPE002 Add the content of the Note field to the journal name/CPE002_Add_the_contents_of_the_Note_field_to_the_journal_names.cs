//C6#CPE002
//C5#43122
//Description: Add Notes To Periodical Name
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

			var periodical = citation.Reference.Periodical;
			if (periodical == null) return null;
			var notesString = periodical.Notes;
			if (string.IsNullOrWhiteSpace(notesString)) return null;

			var periodicalFieldElement = componentPart.GetFieldElements().FirstOrDefault<FieldElement>(item => item.PropertyId == ReferencePropertyId.Periodical);
			if (periodicalFieldElement == null) return null;

			var output = componentPart.GetTextUnitsUnfiltered(citation, template);

			var notesPrefixTextUnit = new LiteralTextUnit(" [");
			notesPrefixTextUnit.FontStyle = FontStyle.Neutral;

			var notesTextUnit = new LiteralTextUnit(notesString);
			notesTextUnit.FontStyle = FontStyle.Neutral;
			
			var notesSuffixTextUnit = new LiteralTextUnit("]");
			notesSuffixTextUnit.FontStyle = FontStyle.Neutral;

			output.Add(notesPrefixTextUnit);
			output.Add(notesTextUnit);
			output.Add(notesSuffixTextUnit);

			handled = true;
			return output;

		}
	}
}