//C6#COT024
//C5#431510
//Description: Add prefix "Online verfügbar unter " or "Online available at " and a suffix if needed to Online Address field ("Online-Adresse") depending on language of reference
//Version: 1.1:  Improved handling if the Online Address field is empty


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

			#region Find electronic address field elements

			//we treat only electronic address field elemements that output the Edition field
			IEnumerable<ElectronicAddressFieldElement> electronicAddressFieldElements = componentPart.Elements.OfType<ElectronicAddressFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.OnlineAddress).ToList();
			if (electronicAddressFieldElements == null || !electronicAddressFieldElements.Any()) return null;

			#endregion Find electronic address field elements

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

			if (string.IsNullOrEmpty(reference.OnlineAddress)) return null;

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

			foreach (ElectronicAddressFieldElement element in electronicAddressFieldElements)
			{
				var property = element.PropertyId;
				var value = (string)reference.GetValue(property);

				//if (string.IsNullOrEmpty(value)) continue;
				
				switch (language)
				{
					case (Language.English):
					{
						//add prefix
						LiteralElement newElement = new LiteralElement(componentPart, "Online available at ");
						componentPart.Elements.InsertIElementBefore(element, newElement);
						

						//add suffix
						newElement = new LiteralElement(componentPart, "");
						componentPart.Elements.InsertIElementAfter(element, newElement);
					}
					break;
					
					default:
					case (Language.German):
					{
						//add prefix
						LiteralElement newElement = new LiteralElement(componentPart, "Online verfügbar unter ");
						componentPart.Elements.InsertIElementBefore(element, newElement);
						

						//add suffix
						newElement = new LiteralElement(componentPart, "");
						componentPart.Elements.InsertIElementAfter(element, newElement);					
					}
					break;
					
					case Language.Other:
					{
						//add prefix
						LiteralElement newElement = new LiteralElement(componentPart, "Online verfügbar unter ");
						componentPart.Elements.InsertIElementBefore(element, newElement);
						

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
