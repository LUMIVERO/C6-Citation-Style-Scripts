//CDA007
//Format date as 12th January
//Version 1.1 ordinal suffixes st nd rd th are now formatted as superscripts
//
using System;
using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;
using System.Globalization;

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

			handled = false;
			
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;
			
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
			
			DateTimeFieldElement dateTimeFieldElement = componentPart.Elements.OfType<DateTimeFieldElement>().FirstOrDefault();
			if (dateTimeFieldElement == null) return null;
			
			var propertyId = dateTimeFieldElement.PropertyId;
			var dateTimeStringValue = referenceInScope.GetValue(propertyId) as string;
			if (string.IsNullOrEmpty(dateTimeStringValue)) return null;
			
			DateTime dateValue;
			if (!DateTimeInformation.TryParse(dateTimeStringValue, out dateValue)) return null;
			
			var formattedDate = string.Format(new MyCustomDateProvider(), "{0}", dateValue);
			if (string.IsNullOrEmpty(formattedDate)) return null;
			
			var formattedDateTextUnits = TextUnitCollectionUtility.TaggedTextToTextUnits(null, formattedDate);
			if (formattedDateTextUnits == null || !formattedDateTextUnits.Any()) return null;
			
			if (dateTimeFieldElement.FontStyle != FontStyle.Neutral)
			{
				foreach(ITextUnit textUnit in formattedDateTextUnits)
				{
					textUnit.FontStyle |= dateTimeFieldElement.FontStyle;	
				}
			}
			
			List<LiteralElement> outputDateLiteralElements = formattedDateTextUnits.TextUnitsToLiteralElements(componentPart);
			componentPart.Elements.ReplaceItem(dateTimeFieldElement, outputDateLiteralElements);
			
			foreach(LiteralElement literalElement in componentPart.Elements.OfType<LiteralElement>())
			{
				literalElement.ApplyCondition = ElementApplyCondition.Always;
			}
			
			return null;
		}
		
		public class MyCustomDateProvider : IFormatProvider, ICustomFormatter
		{
		    public object GetFormat(Type formatType)
		    {
		        if (formatType == typeof(ICustomFormatter))
		            return this;

		        return null;
		    }

		    public string Format(string format, object arg, IFormatProvider formatProvider)
		    {
		        if (!(arg is DateTime)) throw new NotSupportedException();
				
				CultureInfo targetCulture = CultureInfo.CreateSpecificCulture("en-US");
		        var dt = (DateTime) arg;
		        string suffix;

		        if (new[] {11, 12, 13}.Contains(dt.Day))
		        {
		            suffix = "<sup>th</sup>";
		        }
		        else if (dt.Day % 10 == 1)
		        {
		            suffix = "<sup>st</sup>";
		        }
		        else if (dt.Day % 10 == 2)
		        {
		            suffix = "<sup>nd</sup>";
		        }
		        else if (dt.Day % 10 == 3)
		        {
		            suffix = "<sup>rd</sup>";
		        }
		        else
		        {
		            suffix = "<sup>th</sup>";
		        }

		        return string.Format(targetCulture, "{1}{2} {0:MMMM}", arg, dt.Day, suffix);
		    }
		}
	}
}
