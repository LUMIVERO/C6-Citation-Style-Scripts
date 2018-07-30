// #C5_431614
// Version: 1.0
// Output number of cites in text or footnote (except: bibonly, yearonly)

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
			var citationManager = citation.CitationManager;
			if (citationManager == null) return null;
			
			var bibliographyCitation = citation as BibliographyCitation;
			if (bibliographyCitation == null) return null;
						
			int countCites = (
				from cite in citationManager.PlaceholderCitations
				where cite.YearOnly == false && cite.BibOnly == false && cite.Reference == citation.Reference
				select cite
			).Count();
			
			if (countCites == 0) return null;
			
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;
		
			LiteralElement countCitesLiteralElement = componentPart.Elements.OfType<LiteralElement>().FirstOrDefault(element => element.Text == "CITE COUNT");
			if (countCitesLiteralElement == null) return null;
			
			countCitesLiteralElement.Text = countCites.ToString();
			handled = false;
			return null;
		}
	}
}