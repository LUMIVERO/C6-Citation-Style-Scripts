//C6#COT002
//C5#4316101
//Description: Add prefix "no." or "Nr." to Number field depening on language of reference
//Version: 1.1 Added ToList() for getting field elements  

using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
			var numericFieldElements = componentPart.Elements.OfType<NumericFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.Number);
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
			
			Language language = Language.German;
			
			var wordListEN = new string[] {
				"EN", 
				"ENG",
				"ENGL",
				"English", 
				"Englisch", 
				"Anglais" 
			};
			
			var regExEN = new Regex(@"\b(" + string.Join("|", wordListEN) + @")\b", RegexOptions.IgnoreCase);
			if (regExEN.IsMatch(reference.Language)) language = Language.English;
			
			#endregion Determine reference language
			
			foreach(NumericFieldElement element in numericFieldElements)
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
							element.SingularPrefix.Text = "no. ";
							element.PluralPrefix.Text = "no. ";
						}
						break;
							
						case Language.German:
						{
							element.SingularPrefix.Text = "Nr. ";
							element.PluralPrefix.Text = "Nr. ";
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
