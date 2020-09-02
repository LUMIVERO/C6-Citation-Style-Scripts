//COT035 Output Reference Type

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
			//return handled = true if this macro generates the output (as an IEnumerable<ITextUnit>); the standard output will be suppressed
			//return handled = false if you want Citavi to produce the standard output;

			handled = true;

			TextUnitCollection output = new TextUnitCollection();
			LiteralTextUnit text;

			string referenceType = citation.Reference.ReferenceType.ToString();

			text = new LiteralTextUnit(referenceType, FontStyle.Neutral);
		//	text = new LiteralTextUnit(referenceType, FontStyle.Bold | FontStyle.SmallCaps);

			output.Add(new LiteralTextUnit("[", FontStyle.Neutral));
			output.Add(text);
			output.Add(new LiteralTextUnit("]", FontStyle.Neutral));
			
			return output;
		}
	}
}
