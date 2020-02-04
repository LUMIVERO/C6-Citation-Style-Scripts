//C6#COT030
//Description: Output LANGUAGE field depending on language of reference
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

			#region Find field elements, of type: TextFieldElement, ReferencePropertyId = Language

			var languageFieldElements = componentPart.Elements.OfType<TextFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.Language).ToList();
			if (languageFieldElements == null || !languageFieldElements.Any()) return null;

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

			foreach (TextFieldElement element in languageFieldElements)
			{
				var property = element.PropertyId;
				var value = (string)reference.GetValue(property);

				//if (string.IsNullOrEmpty(value)) continue;
				
				switch (language)
				{
					case (Language.English):
					{
						//no output
						handled = true;
						return null;
					}
					break;
					
					default:
					case (Language.German):
					{
						//output the Language
						handled = false;
						return null;					
					}
					break;
					
					case Language.Other:
					{
						//output the Language
						handled = false;
						return null;
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
