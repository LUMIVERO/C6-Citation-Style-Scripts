//C6#CDA010
//Description: Eckige Klammern vor & nach "Erstveröffentlichung" entfernen, wenn der Wert für "Jahr ermittelt" leer ist
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
			
			var hasOriginalPublication = !string.IsNullOrEmpty(citation.Reference.OriginalPublication);		//Erstveröffentlichung
			var hasYearResolved = !string.IsNullOrEmpty(citation.Reference.YearResolved);					//Jahr ermittelt
			
			if (!hasOriginalPublication && !hasYearResolved) return null; //die Komponente "leert" sich von alleine, denn die (statische) Beschriftung wird nur ausgegeben, wenn eines der Felder Inhalt hat.
			
			
			IEnumerable<DateTimeFieldElement> originalPublicationFieldElements = componentPart.Elements.OfType<DateTimeFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.OriginalPublication);
			if (originalPublicationFieldElements == null || originalPublicationFieldElements.Count() != 1) return null;
			DateTimeFieldElement originalPublicationFieldElement = originalPublicationFieldElements.FirstOrDefault();
			if (originalPublicationFieldElement == null) return null;

			IEnumerable<DateTimeFieldElement> yearResolvedFieldElements = componentPart.Elements.OfType<DateTimeFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.YearResolved);
			if (yearResolvedFieldElements == null || yearResolvedFieldElements.Count() != 1) return null;
			DateTimeFieldElement yearResolvedFieldElement = yearResolvedFieldElements.FirstOrDefault();
			if (yearResolvedFieldElement == null) return null;
			
			IEnumerable<LiteralElement> literalElements = componentPart.Elements.OfType<LiteralElement>().Where(element => element.Text == " [" || element.Text == "]");
			if (literalElements == null || literalElements.Count() != 2) return null;
			LiteralElement literalElement1 = literalElements.FirstOrDefault();
			LiteralElement literalElement2 = literalElements.Last();
			if (literalElement1 == null) return null;
			if (literalElement2 == null) return null;

			//nur hasOriginalPublication -> eckige Klammern vor & nach "Erstveröffentlichung" sollen entfallen
			if (hasOriginalPublication && !hasYearResolved)
			{
				literalElement1.Text = "";
				literalElement2.Text = "";
				return null;
			}
			
			//nur hasYearResolved? -> alles bleibt, wie es ist
			if (!hasOriginalPublication && hasYearResolved)
			{
				return null;
			}
			
			//hasOriginalPublication UND hasYearResolved? -> alles bleibt, wie es ist
			if (hasOriginalPublication && hasYearResolved)
			{
			//	componentPart.Elements.Remove(hasYearResolved);		falls nur das Erstveröffentlichungsjahr ausgegeben werden soll
			//	literalElement1.Text = "";
			//	literalElement2.Text = "";	
				return null;
			}
			
			
			//hierher sollten wir nie kommen, aber wer weiss
			return null;
			
		}
	}
}
