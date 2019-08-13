//C6#COT018
//C5#431526
//Description: Force to use prefixes
//Version: 1.1, requires Citavi 5.3 and above
//Bei "Unverändert (wie Eingabe)" [C5] bzw. "Kein Zahlensystem (Ausgabe wie Eingabe)" [C6]:
//+Präfix, +Suffix, +Bindestrich durch Halbgeviertstrich (Gedankenstrich) ersetzen
//Funktionsweise: Das ERSTE PageRangeFieldElement wird durch korrigierten Output ersetzt.
//Das Skript kann auch in einer kombinierten Komponente mit weiteren Elementen verwendet werden.
//Version: 1.1 changed mode of script from handling the output itself to changing the component part and let Citavi do the rest (lines 115 ff.) 

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
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null) return null;
			
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			
			
			PageRangeFieldElement pageRangeFieldElement = componentPart.Elements.OfType<PageRangeFieldElement>().FirstOrDefault();
			if (pageRangeFieldElement == null) return null;
				
			
			PageRange pageRangeResolved = PageRange.Empty;
			if (pageRangeFieldElement is QuotationPageRangeFieldElement)
			{							
				PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
				if (placeholderCitation == null) return null;
				if (placeholderCitation.Entry == null) return null;
				pageRangeResolved = placeholderCitation.Entry.PageRange;	
			}
			else
			{
				pageRangeResolved = citation.Reference.PageRange;
			}	
			
			if (pageRangeResolved == null) return null;
			if (pageRangeResolved == PageRange.Empty) return null;			
			if (pageRangeResolved.NumeralSystem != NumeralSystem.Omit) return null;
			
			LiteralElement prefix = null;
			LiteralElement suffix = null;
			switch (pageRangeResolved.NumberingType)
			{
				case NumberingType.Column:
					prefix = pageRangeFieldElement.ColumnMultiPrefix;
					suffix = pageRangeFieldElement.ColumnMultiSuffix;
					break;
					
				case NumberingType.Margin:
					prefix = pageRangeFieldElement.MarginMultiPrefix;
					suffix = pageRangeFieldElement.MarginMultiSuffix;
					break;
					
				case NumberingType.Other:
					prefix = pageRangeFieldElement.OtherMultiPrefix;
					suffix = pageRangeFieldElement.OtherMultiSuffix;
					break;
				
				case NumberingType.Page:
					prefix = pageRangeFieldElement.PageMultiPrefix;
					suffix = pageRangeFieldElement.PageMultiSuffix;
					break;
				
				case NumberingType.Paragraph:
					prefix = pageRangeFieldElement.ParagraphMultiPrefix;
					suffix = pageRangeFieldElement.ParagraphMultiSuffix;
					break;
			}
			
			bool hasPrefix = prefix != null && !string.IsNullOrEmpty(prefix.Text);
			bool hasSuffix = suffix != null && !string.IsNullOrEmpty(suffix.Text);
			
			
		
			TextUnitCollection pageRangeTextUnits = pageRangeFieldElement.GetTextUnits(citation, template);
			if (pageRangeTextUnits == null || !pageRangeTextUnits.Any()) return null;
			
			
			
			foreach(ITextUnit unit in pageRangeTextUnits)
			{
				unit.Text = unit.Text.Replace(System.StringUtility.Divis, System.StringUtility.EnDash);
			}

			TextUnitCollection output = new TextUnitCollection();
			
			if (hasPrefix && pageRangeTextUnits.First().Text != prefix.Text)
			{
				TextUnitCollection prefixTextUnits = prefix.GetTextUnits(citation, template);
				if (prefixTextUnits != null && prefixTextUnits.Any()) output.AddRange(prefixTextUnits);
			}
				
			output.AddRange(pageRangeTextUnits);
			
			if (hasSuffix && pageRangeTextUnits.Last().Text != suffix.Text)
			{
				TextUnitCollection suffixTextUnits = suffix.GetTextUnits(citation, template);
				if (suffixTextUnits != null && suffixTextUnits.Any()) output.AddRange(suffixTextUnits);
			}
			
			componentPart.Elements.ReplaceItem(pageRangeFieldElement, TextUnitCollectionUtility.TextUnitsToLiteralElements(output, componentPart));
			handled = false;
			
			//all literal elements should always be output:
			foreach (LiteralElement literalElement in componentPart.Elements.OfType<LiteralElement>())
			{
				literalElement.ApplyCondition = ElementApplyCondition.Always;
			}
			
			return null;
		}
	}
}
