//COT022
//Description:  Only manually modified citation keys will be displayed, the output of automatically generated citation keys will be suppressed
//Version 1.1:	Added option to output the title of the reference, if the citation key is NOT manually set
//Version 1.0:	Citavi 5+

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
			var titleAsFallback = true;					//if titleAsFallback = true, the title will be displayed, if the citation key is NOT manually set
														//if titleAsFallback = false, nothing will be displayed, if the citation key is NOT manually set
														//in both cases, the citation key will be displayed, if it is manually set
			
			//suppresses the output of the component, if a CitationKey field element is present and if the current citation key was automatically created
			handled = false;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null) return null;
			
			if (citation == null || citation.Reference == null) return null;
			if (componentPart.Scope == ComponentPartScope.ParentReference && citation.Reference.ParentReference == null) return null;
			
			
			var citationKeyFieldElement = componentPart.Elements.OfType<CitationKeyFieldElement>().FirstOrDefault();
			if (citationKeyFieldElement == null) return null;

			//
			TextUnitCollection output = new TextUnitCollection();
			LiteralTextUnit text;
			//
			
			string citationKeyResolved;
			UpdateType citationKeyUpdateTypeResolved;
			if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				citationKeyResolved = citation.Reference.ParentReference.CitationKey;
				citationKeyUpdateTypeResolved = citation.Reference.ParentReference.CitationKeyUpdateType;
			}
			else
			{
				citationKeyResolved = citation.Reference.CitationKey;
				citationKeyUpdateTypeResolved = citation.Reference.CitationKeyUpdateType;
			}
			
			if (citationKeyUpdateTypeResolved == UpdateType.Automatic || string.IsNullOrWhiteSpace(citationKeyResolved))
			{
				if (titleAsFallback)
				{
					handled = true;
					text = new LiteralTextUnit(citation.Reference.Title, FontStyle.Neutral);

					output.Add(text);
					
					return output;
				}
				else
				{
					handled = true;
					return null;
				}
			}
			
			return null;
		}
	}
}
