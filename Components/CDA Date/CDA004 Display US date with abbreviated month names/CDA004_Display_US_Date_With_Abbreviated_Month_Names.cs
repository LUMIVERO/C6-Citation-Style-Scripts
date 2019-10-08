//#43143
//Version 2.3
//US date with short month name, e.g. 15 May 2015 or 7 Jan. 2011

using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
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
		//Version 2.3 introduction of debug mode
		//Version 2.2 added parameter 'doNotAbbreviateIfLengthIsEqualToOrLess' to avoid abbreviation of June and July (=4) 
		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			bool debugMode = false; //set to true to see, what date Citavi recognized
			
			handled = false;
		
			if (componentPart == null) return null;
			if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			if (citation == null || citation.Reference == null) return null;
			
			Reference referenceInScope = null;
			if (componentPart.Scope == ComponentPartScope.Reference)
			{
				referenceInScope = citation.Reference;
			}
			else if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				referenceInScope = citation.Reference.ParentReference;
			}
			if (referenceInScope == null) return null;
			
			
			IEnumerable<DateTimeFieldElement> dateFieldElements = componentPart.Elements.OfType<DateTimeFieldElement>().ToList();
			if (dateFieldElements == null || !dateFieldElements.Any()) return null;
	

			bool dateFound = false;
			
			CultureInfo targetCulture = CultureInfo.CreateSpecificCulture("en-US");
			
			int doNotAbbreviateIfLengthIsEqualToOrLess = 4;		
			//put 4 for most English styles: 
			//3: Jan[uary]=7, Feb[ruary]=8, Mar[ch]=5, Apr[il]=5, May=3, Jun[e]=4, Jul[y]=4, Aug[ust]=6, Sep[tember]=9, Oct[ober]=7, Nov[ember]=8, Dec[ember]=8
			//4: Jan[uary]=7, Feb[ruary]=8, Mar[ch]=5, Apr[il]=5, May=3, June=4, July=4, Aug[ust]=6, Sep[tember]=9, Oct[ober]=7, Nov[ember]=8, Dec[ember]=8
			
			foreach (DateTimeFieldElement dateFieldElement in dateFieldElements)
			{
				string dateString = referenceInScope.GetValue(dateFieldElement.PropertyId) as string;
				if (string.IsNullOrEmpty(dateString)) continue;
				
				DateTime dateValue;
				if (!DateTimeInformation.TryParse(dateString, out dateValue)) continue;

				int day = dateValue.Day;
				int month = dateValue.Month;
				int year = dateValue.Year;

				string monthStringShort = dateValue.ToString("MMM", targetCulture);
				string monthStringLong = dateValue.ToString("MMMM", targetCulture);

				string monthString = monthStringShort == monthStringLong ? monthStringShort : monthStringLong.Length <= doNotAbbreviateIfLengthIsEqualToOrLess ? monthStringLong : monthStringShort + ".";
				//string dayString = day.ToString("D2");				//2-digit day, padded with leading 0 if necessary, so 08 instead of 8
				string dayString = day.ToString();
				string yearString = dateValue.ToString("yyyy");			//4-digit year

				if (debugMode)
				{
					string debugDatePattern = "Original: {0} - Year: {1} Month: {2} Day: {3}";
					dateString = string.Format(debugDatePattern, dateString, dateValue.Year, dateValue.Month, dateValue.Day);
				}
				else
				{
					string newDatePattern = "{0} {1} {2}";
					dateString = string.Format(newDatePattern, dayString, monthString, yearString);
				}
				
				LiteralElement outputDateElement = new LiteralElement(componentPart, dateString);
				outputDateElement.FontStyle = dateFieldElement.FontStyle;
				componentPart.Elements.ReplaceItem(dateFieldElement, outputDateElement);
				dateFound = true;
			}
		
			if (dateFound)
			{
				foreach(LiteralElement literalElement in componentPart.Elements.OfType<LiteralElement>())
				{
					literalElement.ApplyCondition = ElementApplyCondition.Always;
				}
			}
			
			handled = false;
			return null;
		}
	}
}
