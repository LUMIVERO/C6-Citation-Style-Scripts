//C6#CED003
//Description: Add suffix "Aufl." or "ed." and a prefix if needed to edition field depending on language of reference
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

			#region Find numeric field elements

			//we treat only numeric field elemements that output the Edition field
			var numericFieldElements = componentPart.Elements.OfType<NumericFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.Edition).ToList();
			if (numericFieldElements == null || !numericFieldElements.Any()) return null;

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

			foreach (NumericFieldElement element in numericFieldElements)
			{
				var property = element.PropertyId;
				var value = (string)reference.GetValue(property);

				if (string.IsNullOrEmpty(value)) continue;

				int number;
				bool isNumeric = int.TryParse(value, out number);

				#region Edition field contains just a number

				if (isNumeric)
				{
					switch (language)
					{
						case Language.English:
							{							
								element.UseNumericAbbreviations = true;
								element.DefaultNumericAbbreviation.Text = "th";
								element.DefaultNumericAbbreviation.FontStyle = FontStyle.Superscript;
								
								element.SpecialNumericAbbreviations.Text = "1|st|2|nd|3|rd";
								element.SpecialNumericAbbreviations.FontStyle = FontStyle.Superscript;
								
								//to avoid the Superscript propagating any further, we set the font style to neutral
								element.SingularSuffix.Text = " ed.";
								element.SingularSuffix.FontStyle = FontStyle.Neutral;
								
								element.PluralSuffix.Text = " ed.";
								element.PluralSuffix.FontStyle = FontStyle.Neutral;
							}
							break;

						default:
						case Language.German:
							{
								element.SingularSuffix.Text = ". Aufl.";
								element.PluralSuffix.Text = ". Aufl.";
							}
							break;
							
						case Language.Other:
							{
								element.SingularSuffix.Text = ". Aufl.";
								element.PluralSuffix.Text = ". Aufl.";
							}
							break;
					}
				}

				#endregion Edition field contains just a number

				#region Edition field contains text

				else
				{
					switch (language)
					{
						case Language.English:
							{
								element.SingularSuffix.Text = "";
								element.PluralSuffix.Text = "";
							}
							break;

						default:
						case Language.German:
							{
								element.SingularSuffix.Text = "";
								element.PluralSuffix.Text = "";
							}
							break;
							
						case Language.Other:
							{
								element.SingularSuffix.Text = "";
								element.PluralSuffix.Text = "";
							}
							break;
					}
				}

				#endregion Edition field contains text
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
