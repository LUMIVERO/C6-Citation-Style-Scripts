//COT023
//Citavi 6.3.11+
//Description: Citation key will only be displayed to avoid ambiguous references in the text or within footnotes
//Version 1.2: Modified for output in the bibliography, since AmbiguityFound and AmbiguityResolved properties don't need to be determined by iterating through citation any more with Citavi 6.3.11
//Version 1.1: Added: Output of the citation key in the bibliography, if it was previously used to resolve ambiguous references in the text or within footnotes

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
			handled = true;
			
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			
			if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			if (componentPart.Elements.Count != 1) return null;
				
			CitationKeyFieldElement citationKeyFieldElement = componentPart.Elements.ElementAt(0) as CitationKeyFieldElement;
			if (citationKeyFieldElement == null) return null;
			
			
			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation != null)
			{
				if (placeholderCitation.CorrespondingBibliographyCitation == null) return null;
				if (placeholderCitation.IsAmbiguityTest) return null;

				if (placeholderCitation.AmbiguityFound && !placeholderCitation.AmbiguityResolved)
				{
					handled = false; //this will display the citation key
					return null;
				}
			}

			if (citation.CitationManager == null) return null;
			
			BibliographyCitation bibliographyCitation = citation as BibliographyCitation;
			if (bibliographyCitation != null)
			{
				if (bibliographyCitation.AmbiguityFound && !bibliographyCitation.AmbiguityResolved)
				{
					handled = false; //this will display the citation key
					return null;
				}
			}

			
			return null;
		}
	}
}
