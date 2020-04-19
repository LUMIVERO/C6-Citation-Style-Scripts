//CDA002

using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;
using System;


namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
		:
		IComponentPartFilter
	{

		//Version 2.0 script can be attached to both date/time field element as well as text field element
		
		//Format date range (if applicable)
		//Enter date range as: dd.MM.yyyy - dd.MM.yyyy
		//Format it to MMM. dd-dd, yyyy e.g.

		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			CultureInfo targetCulture = CultureInfo.CreateSpecificCulture("en-US");

			handled = false;

			if (citation == null) return null;
			if (citation.Reference == null) return null;

			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count != 1) return null;

			FieldElement firstDateContainingFieldElement = componentPart.Elements.Where(element => 
			{
				DateTimeFieldElement dateTimeFieldElement = element as DateTimeFieldElement;
				if (dateTimeFieldElement != null) return true;
				
				TextFieldElement textFieldElement = element as TextFieldElement;
				if (textFieldElement != null) return true;
				
				return false;
			}).FirstOrDefault() as FieldElement;
			if (firstDateContainingFieldElement == null) return null;
			
			ReferencePropertyId referencePropertyId = firstDateContainingFieldElement.PropertyId;
			
			Reference referenceResolved = componentPart.Scope == ComponentPartScope.ParentReference ? citation.Reference.ParentReference : citation.Reference;
			if (referenceResolved == null) return null;

			string date = referenceResolved.GetValue(referencePropertyId) as string;
			if (string.IsNullOrEmpty(date)) return null;

			FontStyle fontStyle = 
				firstDateContainingFieldElement is DateTimeFieldElement ? 
				((DateTimeFieldElement)firstDateContainingFieldElement).FontStyle :
				((TextFieldElement)firstDateContainingFieldElement).FontStyle;

			DateTime dateSingle;
			DateTime dateA;
			DateTime dateB;

			CultureInfo deDE = new CultureInfo("de-DE");
			string[] formats = new string[] { "yyyy-MM-dd", "dd.MM.yyyy", "d.M.yyyy", "d.MM.yyyy", "dd.M.yyyy", "dd.MM.yy", "d.M.yy", "d.MM.yy", "dd.M.yy" };


			//try single date first
			var found = DateTime.TryParseExact(date, formats, deDE, DateTimeStyles.None, out dateSingle);

			if (found)
			{
				var monthStringShort = dateSingle.ToString("MMM", targetCulture);
				var monthStringLong = dateSingle.ToString("MMMM", targetCulture);
				var monthString = monthStringShort == monthStringLong ? monthStringShort : monthStringShort + ".";

				var yearString = dateSingle.ToString("yyyy");

				var dayString = dateSingle.Day.ToString("D2");

				var outputFormatSingle = "{0} {1} {2}";
				var dateSingleText = string.Format(outputFormatSingle, dayString, monthString, yearString);

				var outputSingleDate = new TextUnitCollection();
				var textSingleDate = new LiteralTextUnit(dateSingleText);
				textSingleDate.FontStyle = fontStyle;
				outputSingleDate.Add(textSingleDate);
				handled = true;
				return outputSingleDate;

			}

			//then try date range
			List<string> dates = date.Split('-').Select(d => d.Trim()).ToList();
			if (dates.Count != 2) return null;


			var foundA = DateTime.TryParseExact(dates.ElementAt(0), formats, deDE, DateTimeStyles.None, out dateA);
			var foundB = DateTime.TryParseExact(dates.ElementAt(1), formats, deDE, DateTimeStyles.None, out dateB);

			if (!foundA || !foundB) return null;


			var monthAStringShort = dateA.ToString("MMM", targetCulture);
			var monthAStringLong = dateA.ToString("MMMM", targetCulture);
			var monthAString = monthAStringShort == monthAStringLong ? monthAStringShort : monthAStringShort + ".";

			var monthBStringShort = dateB.ToString("MMM", targetCulture);
			var monthBStringLong = dateB.ToString("MMMM", targetCulture);
			var monthBString = monthBStringShort == monthBStringLong ? monthBStringShort : monthBStringShort + ".";

			var yearAString = dateA.ToString("yyyy");
			var yearBString = dateB.ToString("yyyy");

			var dayAString = dateA.Day.ToString("D2");
			var dayBString = dateB.Day.ToString("D2");

			string outputFormat = string.Empty;
			string dateRangeText = string.Empty;

			//same year, same month
			if (dateA.Year == dateB.Year && dateA.Month == dateB.Month && dateA.Day != dateB.Day)
			{
				outputFormat = "{0} {1}-{2}, {3}"; //e.g. Sep. 08-11, 2013
				dateRangeText = string.Format(outputFormat, monthAString, dayAString, dayBString, yearAString);
			}


			//same year, different months
			else if (dateA.Year == dateB.Year && dateA.Month != dateB.Month)
			{
				outputFormat = "{0} {1} - {2} {3}, {4}"; //e.g. Sep. 27 - Oct. 04, 2013
				dateRangeText = string.Format(outputFormat, monthAString, dayAString, monthBString, dayBString, yearAString);
			}

			//different years
			else
			{
				outputFormat = "{0} {1}, {2} - {3} {4}, {5}"; //e.g. Dec. 27, 2013 - Jan. 04, 2014
				dateRangeText = string.Format(outputFormat, monthAString, dayAString, yearAString, monthBString, dayBString, yearBString);
			}

			var output = new TextUnitCollection();
			var text = new LiteralTextUnit(dateRangeText);
			text.FontStyle = fontStyle;
			output.Add(text);
			handled = true;
			return output;
		}
	}
}
