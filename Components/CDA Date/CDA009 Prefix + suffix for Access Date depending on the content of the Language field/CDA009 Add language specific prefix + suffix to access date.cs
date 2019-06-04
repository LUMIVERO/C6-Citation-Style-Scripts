//C6#CDA009
//C5#431510
//Description: Add prefix "Zuletzt geprüft am " or "Last access " to Access date field ("Zuletzt geprüft am") depending on language of reference
//Version: 1.0  


using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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

			if (citation == null) return null;
			if (citation.Reference == null) return null;
			if (componentPart == null) return null;

			#region Find date time field elements

			//we treat only numeric field elemements that output the Edition field
			var dateTimeFieldElements = componentPart.Elements.OfType<DateTimeFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.AccessDate).ToList();
			if (dateTimeFieldElements == null || !dateTimeFieldElements.Any()) return null;

			#endregion Find numeric field elements

			#region Determine reference to look at

			Reference reference;
			if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				if (citation.Reference.ParentReference == null) return null;
				reference = citation.Reference.ParentReference as Reference;
			}
			else
			{
				reference = citation.Reference as Reference;
			}
			if (reference == null) return null;

			#endregion Determine reference to look at

			#region Determine reference language

			Language language;
			if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("en").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.English;
			}
			else if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("de").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.German;
			}
			else
			{
				language = Language.Other;
			}

			#endregion Determine reference language

			foreach (DateTimeFieldElement element in dateTimeFieldElements)
			{
				var property = element.PropertyId;
				var value = (string)reference.GetValue(property);
				
				if (string.IsNullOrEmpty(value)) continue;
				
				switch (language)
				{
					case Language.English:
					{
						//add prefix
						LiteralElement newElement = new LiteralElement(componentPart, "Last access ");
						componentPart.Elements.InsertIElementBefore(element, newElement);
						
						element.Format = "yyyy-MM-dd";
						
						//add suffix
						newElement = new LiteralElement(componentPart, "");
						componentPart.Elements.InsertIElementAfter(element, newElement);
					}
					break;
					
					default:
					case Language.German:
					{
						//add prefix
						LiteralElement newElement = new LiteralElement(componentPart, "Zuletzt geprüft am ");
						componentPart.Elements.InsertIElementBefore(element, newElement);
						
						element.Format = "dd.MM.yyyy";
						
						//add suffix
						newElement = new LiteralElement(componentPart, "");
						componentPart.Elements.InsertIElementAfter(element, newElement);					
					}
					break;
					
					case Language.Other:
					{
						//add prefix
						LiteralElement newElement = new LiteralElement(componentPart, "Zuletzt geprüft am ");
						componentPart.Elements.InsertIElementBefore(element, newElement);
						
						element.Format = "dd.MM.yyyy";
						
						//add suffix
						newElement = new LiteralElement(componentPart, "");
						componentPart.Elements.InsertIElementAfter(element, newElement);
					}
					break;
				}				
			}

			return null;
		}

		private enum Language
		{
			English,
			German,
			Other
		}
	}
}
