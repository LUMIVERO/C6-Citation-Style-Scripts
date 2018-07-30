// #C5_431619
//Version 1.0
//Output letter to resolve ambiguity
//This component part filter outputs the letter used for resolving ambiguity (IF the style uses letters for resolving ambiguity AND the current citation is ambiguous)
//It looks either at the current citation's reference OR its parent reference, depending on the scope of the component part.
//IMPORTANT: This filter requires an empty literal element inside the component part it is attached to. It will replace this empty literal element by the ambiguity resolving letter.

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
			
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			
			if (componentPart == null) return null;
			if (componentPart.Scope == ComponentPartScope.ParentReference && citation.Reference.ParentReference == null) return null;
			
			//check if the placeholder literal element is present and exit if not
			var literalElementToBeReplaced = componentPart.Elements.OfType<LiteralElement>().FirstOrDefault(element => string.IsNullOrEmpty(element.Text));
			if (literalElementToBeReplaced == null) return null;
			
			//try to get a bibliographyCitation for the current citation
			var bibliographyCitation = citation as BibliographyCitation;
			if (bibliographyCitation == null)
			{
				var inTextCitation = citation as InTextCitation;
				if (inTextCitation != null) bibliographyCitation = inTextCitation.CorrespondingBibliographyCitation;
			}
			if (bibliographyCitation == null)
			{
				var footnoteCitation = citation as FootnoteCitation;
				if (footnoteCitation != null) bibliographyCitation = footnoteCitation.CorrespondingBibliographyCitation;
			}
			if (bibliographyCitation == null) return null;
			

			//determine the letter for resolving ambiguity
			string letter = string.Empty;
			if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				if (bibliographyCitation.CitationManager == null) return null;
				var parentBibliographyCitation = bibliographyCitation.CitationManager.BibliographyCitations.FirstOrDefault(entry => entry.Reference == citation.Reference.ParentReference);
				if (parentBibliographyCitation == null) return null;
				letter = parentBibliographyCitation.IdentifyingLetter;
			}
			else
			{
				letter = bibliographyCitation.IdentifyingLetter;
			}	
			if (string.IsNullOrEmpty(letter)) return null;
		
			
			//replace the empty literal element ("placeholder") by a literal lement with the letter 
			
			//new literal element
			var newLiteralElement = new LiteralElement(componentPart, letter);
			newLiteralElement.FontStyle = literalElementToBeReplaced.FontStyle;
			
			componentPart.Elements.ReplaceItem(literalElementToBeReplaced, newLiteralElement);
			
			//Citavi now treats the modified component part
			return null;

		}
	}
}