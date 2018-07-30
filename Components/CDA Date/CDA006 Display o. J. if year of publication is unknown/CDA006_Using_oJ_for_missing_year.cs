//CDA006
//Description: Using o. J. for missing publication year

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
			bool addAmbiguityResolvingLetter = true;
			string noYearString = "o.J.";
			string noYearTemplate = "{0}{1}";	//add a space if you do not want the ambiguity resolving letter to "stick" to the no-year-string: "{0} {1}"

			if (citation == null) return null;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;
         
			Reference reference = citation.Reference;
			if (reference == null) return null;
         
			//Reference in Scope of ComponentPart
			Reference referenceInScope = componentPart.Scope == ComponentPartScope.Reference ? citation.Reference : citation.Reference.ParentReference;
			if (referenceInScope == null) return null;
			
			var yearFieldElement = componentPart.Elements
				.OfType<DateTimeFieldElement>()
				.Where(item => item.PropertyId == ReferencePropertyId.Year || item.PropertyId == ReferencePropertyId.YearResolved)
				.FirstOrDefault() as DateTimeFieldElement;
         
			if (yearFieldElement == null) return null;
         

			string yearValue = referenceInScope.GetValue(yearFieldElement.PropertyId) as string;
			if (!string.IsNullOrEmpty(yearValue)) return null;
		

			//for the identifying letter we need a corresponding bibliography citation
			BibliographyCitation correspondingBibliographyCitationInScope = null;
			BibliographyCitation thisBibliographyCitation = citation as BibliographyCitation;
			if (componentPart.Scope == ComponentPartScope.Reference)
			{
				#region ComponentPartScope.Reference
				
				if (thisBibliographyCitation == null)
				{
					PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
					if (placeholderCitation != null)
					{
						correspondingBibliographyCitationInScope = placeholderCitation.CorrespondingBibliographyCitation;
					}
				}
				else
				{
					correspondingBibliographyCitationInScope = thisBibliographyCitation;
				}
				
				#endregion
			}
			else
			{
				#region ComponentPartScope.ParentReference
				
				if (citation.CitationManager != null)
                {
                    foreach (BibliographyCitation otherBibliographyCitation in citation.CitationManager.BibliographyCitations)
                    {
                        if (otherBibliographyCitation == null) continue;
                        if (otherBibliographyCitation == thisBibliographyCitation) continue;

                        if (otherBibliographyCitation.Reference == null) continue;
                        if (otherBibliographyCitation.Reference == referenceInScope) correspondingBibliographyCitationInScope = otherBibliographyCitation;
                    }
                }
				
				#endregion 
			}
			string identifyingLetter = string.Empty;
			if (correspondingBibliographyCitationInScope != null) identifyingLetter = correspondingBibliographyCitationInScope.IdentifyingLetter;
		
			string outputString = string.Format(noYearTemplate, noYearString, identifyingLetter);
			LiteralElement outputLiteralElement = new LiteralElement(componentPart, outputString);
			outputLiteralElement.FontStyle = yearFieldElement.FontStyle;
			componentPart.Elements.ReplaceItem(yearFieldElement, outputLiteralElement);
		    
			//all literal elements should always be output:
			foreach(LiteralElement literalElement in componentPart.Elements.OfType<LiteralElement>())
			{
				literalElement.ApplyCondition = ElementApplyCondition.Always;
			}

			return null;
		}
	}
}