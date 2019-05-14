//COT023
//Version 1.0, Citavi 6.3.6+
//Citation key will only be displayed, if the output otherwise results in ambiguous references

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
			
			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return null;
			if (placeholderCitation.CorrespondingBibliographyCitation == null) return null;
			
			if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			if (componentPart.Elements.Count != 1) return null;
			
			CitationKeyFieldElement citationKeyFieldElement = componentPart.Elements.ElementAt(0) as CitationKeyFieldElement;
			if (citationKeyFieldElement == null) return null;
			if (placeholderCitation.IsAmbiguityTest) return null;

			if (placeholderCitation.AmbiguityFound && !placeholderCitation.AmbiguityResolved)
			{
				handled = false; //this will display the citation key
			}
			
			return null;
		}
	}
}
