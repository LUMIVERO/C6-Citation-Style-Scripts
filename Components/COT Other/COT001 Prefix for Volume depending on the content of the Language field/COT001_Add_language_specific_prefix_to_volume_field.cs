//C6#COT001
//C5#431510
//Description: Add prefix "Vol." or "Jg." to volume field depending on language of reference
//Version: 1.1 Added ToList() for getting field elements 


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

			#region Find numeric field elements

			//we treat only numeric field elemements that output the Volume field
			var numericFieldElements = componentPart.Elements.OfType<NumericFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.Volume).ToList();
			if (numericFieldElements == null || numericFieldElements.Count() == 0) return null;

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
			if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("de").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.German;
			}
			else
			{
				language = Language.English;
			}

			#endregion Determine reference language

			foreach (NumericFieldElement element in numericFieldElements)
			{
				var property = element.PropertyId;
				var value = (string)reference.GetValue(property);

				if (string.IsNullOrEmpty(value)) continue;

				int number;
				bool isNumeric = int.TryParse(value, out number);

				#region Volume field contains just a number

				if (isNumeric)
				{
					switch (language)
					{
						case Language.English:
							{
								element.SingularPrefix.Text = "Vol. ";
								element.PluralPrefix.Text = "Vol. ";
							}
							break;

						case Language.German:
							{
								element.SingularPrefix.Text = "Jg. ";
								element.PluralPrefix.Text = "Jg. ";
							}
							break;
					}
				}

				#endregion Volume field contains just a number

				#region Volume field contains text

				else
				{
					switch (language)
					{
						case Language.English:
							{
								element.SingularPrefix.Text = "";
								element.PluralPrefix.Text = "";
							}
							break;

						case Language.German:
							{
								element.SingularPrefix.Text = "";
								element.PluralPrefix.Text = "";
							}
							break;
					}
				}

				#endregion Volume field contains text
			}

			return null;
		}

		private enum Language
		{
			English,
			German
		}
	}
}
