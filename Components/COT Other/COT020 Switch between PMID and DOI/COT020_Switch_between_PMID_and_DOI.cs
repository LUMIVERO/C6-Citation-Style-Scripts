//C6#COT020
//C5#431518
//Description: Switch between PMID and DOI  
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
			//return handled = true if this macro generates the output (as a IEnumerable<ITextUnit>); the standard output will be suppressed
			//return handled = false if you want Citavi to produce the standard output; 
			//you can still manipulate the component part and its elements before letting Citavi generate the output with handled = false
			handled = false;
			
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;
			
			var hasDOI = !string.IsNullOrEmpty(citation.Reference.Doi);
			var hasPubMedId = !string.IsNullOrEmpty(citation.Reference.PubMedId);
			
			if (!hasDOI && !hasPubMedId) return null; //die Komponente "leert" sich von alleine, denn die (statische) Beschriftung wird nur ausgegeben, wenn eines der Felder Inhalt hat.
			
			
			IEnumerable<TextFieldElement> doiFieldElements = componentPart.Elements.OfType<TextFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.Doi);
			if (doiFieldElements == null || doiFieldElements.Count() != 1) return null;
			TextFieldElement doiFieldElement = doiFieldElements.FirstOrDefault();
			if (doiFieldElement == null) return null;

			IEnumerable<TextFieldElement> pmidFieldElements = componentPart.Elements.OfType<TextFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.PubMedId);
			if (pmidFieldElements == null || pmidFieldElements.Count() != 1) return null;
			TextFieldElement pmidFieldElement = pmidFieldElements.FirstOrDefault();
			if (pmidFieldElement == null) return null;
			
			IEnumerable<LiteralElement> literalElements = componentPart.Elements.OfType<LiteralElement>().Where(element => element.Text == "Identifier: ");
			if (literalElements == null || literalElements.Count() != 1) return null;
			LiteralElement literalElement = literalElements.FirstOrDefault();
			if (literalElement == null) return null;
			
			
			//nur PubMedID? -> Beschriftung auf "PMID: "
			if (hasPubMedId && !hasDOI)
			{
				literalElement.Text = "PMID: ";
				return null;
			}
			
			//nur DOI? -> Beschriftung auf "DOI: "
			if (!hasPubMedId && hasDOI)
			{
				literalElement.Text = "DOI: ";
				return null;
			}
			
			//PubMedID UND DOI? -> PubMedID ist zu bevorzugen! Beschriftung auf "PMID: " und DOI-Feldelement rauswerfen
			if (hasPubMedId && hasDOI)
			{
				literalElement.Text = "PMID: ";
				componentPart.Elements.Remove(doiFieldElement);
				return null;
			}
			
			
			//hierher sollten wir nie kommen, aber wer weiss
			return null;
			
		}
	}
}