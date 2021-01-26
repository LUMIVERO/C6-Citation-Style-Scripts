//CDA006
//Description:	Display "o. J." if year of publication is unknown & separating the ambiguity resolving letter by a space
//Version 1.3:	Slight improvements (ContainsInPrintInformation instead of StringComparison of InPrintPlaceholderLanguageVersions)
//Version 1.2:	Considers also other reference types for the in-print option, not just journal articles like the built-in condition "BuiltInTemplateCondition.InPrint".
//Version 1.1:	Additionally adds "im Druck" or "in press" and the letter to resolve ambiguity separated by a space
//Version 1.0:	Using "o.J." or "n.d." if the publication year is empty

using System;
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
			
			bool addInPrintNote = true;					//only if set to true you are able to separate the in-print-note and the ambiguity resolving letter by a space, if required.
			bool addInPrintNoteCustom = false;			//only applicable if addInPrintNote = true, if set to false, the output well be as specified on the date/time field element in component
			string noYearString = "o.J.";				//"o.J.", "n.d."
			string noYearTemplate = "{0} {1}";			//add a space if you do not want the ambiguity resolving letter to "stick" to the no-year-string:	"{0} {1}" -> o. J. a
														//remove the space if you want the ambiguity resolving letter to "stick" to the no-year-string:		"{0}{1}"  -> o.J.a
			string inPrintTemplate = "{0} {1}";			//add a space if you do not want the ambiguity resolving letter to "stick" to the in-print-note:	"{0} {1}" -> in press a
														//remove the space if you want the ambiguity resolving letter to "stick" to the in-print-note:		"{0}{1}"  -> in pressa
			string inPrintNoteCustom = "im Druck";		//"im Druck", ", in press"
			
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
			
			string inPrintNoteStandard = yearFieldElement.InPrintReplacement.Text;
			
			string yearValue = referenceInScope.GetValue(yearFieldElement.PropertyId) as string;

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

			string outputString = string.Empty;

			if (string.IsNullOrEmpty(yearValue))
			{
				//"o.J." or "n.d."
				outputString = string.Format(noYearTemplate, noYearString, identifyingLetter);
			}

			else if (addInPrintNote && StringUtility.ContainsInPrintInformation(yearValue))
			{
				if (addInPrintNoteCustom)
				{
					//"im Druck" or "in print"
					outputString = string.Format(inPrintTemplate, inPrintNoteCustom, identifyingLetter);
				}
				else
				{
					//"im Druck" or "in print" as specified on the date/time field element in component
					if (string.IsNullOrEmpty(inPrintNoteStandard)) return null;
					outputString = string.Format(inPrintTemplate, inPrintNoteStandard, identifyingLetter);
				}
			}

			else 
			{
				//if neither "o.J."/"n.d." nor "im Druck"/"in print" applies
				return null;
			}

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
