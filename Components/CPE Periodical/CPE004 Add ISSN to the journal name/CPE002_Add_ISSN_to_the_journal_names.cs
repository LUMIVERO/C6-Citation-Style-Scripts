//C6#CPE004 (CPE002 Variation)
//C5#43122 Variation
//Description: Add Abbreviation 3 (== UserAbbreviation2) To Periodical Name
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
			var abbreviation3String = periodical.UserAbbreviation2;
			if (string.IsNullOrWhiteSpace(abbreviation3String)) return null;

			var periodicalFieldElement = componentPart.GetFieldElements().FirstOrDefault<FieldElement>(item => item.PropertyId == ReferencePropertyId.Periodical);
			if (periodicalFieldElement == null) return null;

			var output = componentPart.GetTextUnitsUnfiltered(citation, template);

			var abbreviation3PrefixTextUnit = new LiteralTextUnit(" [");
			abbreviation3PrefixTextUnit.FontStyle = FontStyle.Neutral;

			var abbreviation3TextUnit = new LiteralTextUnit(abbreviation3String);
			abbreviation3TextUnit.FontStyle = FontStyle.Neutral;
			
			var abbreviation3SuffixTextUnit = new LiteralTextUnit("]");
			abbreviation3SuffixTextUnit.FontStyle = FontStyle.Neutral;

			output.Add(abbreviation3PrefixTextUnit);
			output.Add(abbreviation3TextUnit);
			output.Add(abbreviation3SuffixTextUnit);

			handled = true;
			return output;

		}
	}
}
