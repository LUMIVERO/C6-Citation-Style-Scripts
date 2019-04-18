//COT022
//Version 1.0, Citavi 5+
//Only manually modified citation keys will be displayed, the output of automatically generated citation keys will be suppressed

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
			//suppresses the output of the component, if a CitationKey field element is present and if the current citation key was automatically created
			handled = false;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null) return null;
			
			if (citation == null || citation.Reference == null) return null;
			if (componentPart.Scope == ComponentPartScope.ParentReference && citation.Reference.ParentReference == null) return null;
			
			
			var citationKeyFieldElement = componentPart.Elements.OfType<CitationKeyFieldElement>().FirstOrDefault();
			if (citationKeyFieldElement == null) return null;

			
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
				handled = true;
				return null;
			}
			
			return null;
		}
	}
}
