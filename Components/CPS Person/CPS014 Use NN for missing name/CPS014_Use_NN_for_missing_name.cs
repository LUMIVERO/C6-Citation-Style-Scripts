//C6#CPS019
//C5#43126
//Description: Use N.N. if name is missing
//Version: 1.0 

using System.Linq;
using System.Collections.Generic;
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
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			if (citation == null || citation.Reference == null) return null;
			
			Reference referenceInScope = citation.Reference;
			if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				referenceInScope = citation.Reference.ParentReference;
			}
			if (referenceInScope == null) return null;
			
			
			var missingPersonInfo = "N.N.";                                           	//can be set to e.g. "N.N."; leave empty otherwise
			
			var missingAuthorInfo = "";												//leave empty to always use missingPersonInfo
			var missingEditorInfo = "";												//leave empty to always use missingPersonInfo
			var missingOrganizationInfo = "";										//leave empty to always use missingPersonInfo
			
            var outputMissingPersonsInfoInItalics = false;                              //default: false
            var outputMissingPersonsInfoInSmallCaps = false;                            //default: false
            var outputMissingPersonsInfoInBold = false;                                 //default: false
            var outputMissingPersonsInfoUnderlined = false;                             //default: false
			
			
			IEnumerable<PersonFieldElement> personFieldElements = componentPart.Elements.OfType<PersonFieldElement>().ToList();
			if (!personFieldElements.Any()) return null;
			
			foreach (PersonFieldElement personFieldElement in personFieldElements)
			{
				IEnumerable<Person> persons = personFieldElement.GetPersons(referenceInScope);
				if (!persons.Any())
				{
					#region Determine text to output
					
					string text = missingPersonInfo;
					
					switch (personFieldElement.PropertyId)
					{
						case ReferencePropertyId.Authors:
							text = string.IsNullOrEmpty(missingAuthorInfo) ? missingPersonInfo : missingAuthorInfo;
							break;
							
						case ReferencePropertyId.Editors:
							text = string.IsNullOrEmpty(missingEditorInfo) ? missingPersonInfo : missingEditorInfo;
							break;
							
						case ReferencePropertyId.Organizations:
							text = string.IsNullOrEmpty(missingOrganizationInfo) ? missingPersonInfo : missingOrganizationInfo;
							break;
					}
					
					if (string.IsNullOrEmpty(text)) continue;

					#endregion
					
	                FontStyle fontStyle = SwissAcademic.Drawing.FontStyle.Neutral;
	                if (outputMissingPersonsInfoInItalics) fontStyle |= SwissAcademic.Drawing.FontStyle.Italic;
	                if (outputMissingPersonsInfoInSmallCaps) fontStyle |= SwissAcademic.Drawing.FontStyle.SmallCaps;
	                if (outputMissingPersonsInfoInBold) fontStyle |= SwissAcademic.Drawing.FontStyle.Bold;
	                if (outputMissingPersonsInfoUnderlined) fontStyle |= SwissAcademic.Drawing.FontStyle.Underline;
					
					LiteralElement replacementLiteral = new LiteralElement(componentPart, text);
					replacementLiteral.FontStyle = fontStyle;
					
	                componentPart.Elements.ReplaceItem(personFieldElement, replacementLiteral);
				}
			}
			
			return null;			
		}
	}
}