//CDA003

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

			var dateString = citation.Reference.Date;
			if (string.IsNullOrWhiteSpace(dateString)) return null;

			var dateFieldElement = componentPart.GetFieldElements().FirstOrDefault<FieldElement>(item => item.PropertyId == ReferencePropertyId.Date);
			if (dateFieldElement == null) return null;

			var output = new TextUnitCollection();
			DateTime dateValue;
			if (!DateTime.TryParse(dateString, out dateValue)) return null;

			var day = dateValue.Day; 		//int
			var month = dateValue.Month;	//ditto
			var year = dateValue.Year; 		//ditto

			var monthStringRoman = NumeralSystemConverter.ToRomanNumber(arabicNumber: month.ToString(), lowerCase: false);
			var dayString = day.ToString("D2");				//2-digit day, padded with leading 0 if necessary, so 08 instead of 8
			var yearString = dateValue.ToString("yyyy");	//4-digit year

			var newDatePattern = "{0}. {1} {2}";
			dateString = string.Format(newDatePattern, dayString, monthStringRoman, yearString);

			var dateStringTextUnit = new LiteralTextUnit(dateString);
			output.Add(dateStringTextUnit);

			handled = true;
			return output;

		}
	}
}