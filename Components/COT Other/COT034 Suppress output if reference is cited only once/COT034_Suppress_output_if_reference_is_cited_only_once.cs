//COT034 Suppress output if reference cited only once

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

			
			//return handled = true if this macro generates the output (as an IEnumerable<ITextUnit>); the standard output will be suppressed
			//return handled = false if you want Citavi to produce the standard output; 

			handled = false;
			
			if (citation == null) return null;
			Reference reference = citation.Reference;
			if (reference == null) return null;
			
			if (citation.CitationManager == null) return null;
			CitationManager citationManager = citation.CitationManager;
			
			
			BibliographyCitation bibliographyCitation = citation as BibliographyCitation;
			if (bibliographyCitation == null) return null;
			
			int counter = 0;
			foreach (PlaceholderCitation placeholderCitation in citationManager.PlaceholderCitations)
			{
				if (placeholderCitation == null || placeholderCitation.Reference == null) continue;
				if (placeholderCitation.Reference.Equals(reference)) counter++;
			}
			
			if (counter == 1)
			{
				handled = true;
				return null;
			}
			
			return null;
		}
	}
}
