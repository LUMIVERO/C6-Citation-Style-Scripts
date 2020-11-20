//C6#CPE006
//Description: Add prefix "in: " or "dans : " to Journal Name field ("Zeitschrift") depending on language of reference
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

			#region Find journal field elements

			//we treat only journal field elemements		
			PeriodicalFieldElement periodicalFieldElement = componentPart.Elements.OfType<PeriodicalFieldElement>().FirstOrDefault() as PeriodicalFieldElement;
			if (periodicalFieldElement == null) return null;

			#endregion Find journal field elements

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
			
			var periodical = citation.Reference.Periodical;
			if (periodical == null) return null;
			
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
			else if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("es").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.Spanish;
			}
			else if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("fr").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.French;
			}
			else if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("it").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.Italian;
			}
			else
			{
				language = Language.Other;
			}

			#endregion Determine reference language

		//	var property = element.PropertyId;
		//	var value = (string)reference.GetValue(property);

		//	if (string.IsNullOrEmpty(value)) continue;
				
			switch (language)
			{
				case (Language.English):
				case Language.Spanish:
				case Language.Italian:
				{
					//add prefix
					LiteralElement newElement = new LiteralElement(componentPart, "in: ");
					componentPart.Elements.InsertIElementBefore(periodicalFieldElement, newElement);
					
					//add suffix
					newElement = new LiteralElement(componentPart, "");
					componentPart.Elements.InsertIElementAfter(periodicalFieldElement, newElement);
				}
				break;
				
				case Language.French:
				{
					//add prefix
					LiteralElement newElement = new LiteralElement(componentPart, "dans :\u00A0");
					componentPart.Elements.InsertIElementBefore(periodicalFieldElement, newElement);
					
					//add suffix
					newElement = new LiteralElement(componentPart, "");
					componentPart.Elements.InsertIElementAfter(periodicalFieldElement, newElement);
				}
				break;
				
				default:
				case (Language.German):
				{
					//add prefix
					LiteralElement newElement = new LiteralElement(componentPart, "in: ");
					componentPart.Elements.InsertIElementBefore(periodicalFieldElement, newElement);
					
					//add suffix
					newElement = new LiteralElement(componentPart, "");
					componentPart.Elements.InsertIElementAfter(periodicalFieldElement, newElement);					
				}
				break;
				
				case Language.Other:
				{
					//add prefix
					LiteralElement newElement = new LiteralElement(componentPart, "in: ");
					componentPart.Elements.InsertIElementBefore(periodicalFieldElement, newElement);
					
					//add suffix
					newElement = new LiteralElement(componentPart, "");
					componentPart.Elements.InsertIElementAfter(periodicalFieldElement, newElement);
				}
				break;
			}

			return null;
		}

		private enum Language
		{
			English,
			German,
			Spanish,
			French,
			Italian,
			Other
		}
	}
}
