//COT042

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

			var volumeString = citation.Reference.Volume;
			if (string.IsNullOrWhiteSpace(volumeString)) return null;

			var output = new TextUnitCollection();
			
			var volumeStringRoman = NumeralSystemConverter.ToRomanNumber(arabicNumber: volumeString, lowerCase: true);
			
			var volumeStringTextUnit = new LiteralTextUnit(volumeStringRoman);
			output.Add(volumeStringTextUnit);

			handled = true;
			return output;

		}
	}
}
