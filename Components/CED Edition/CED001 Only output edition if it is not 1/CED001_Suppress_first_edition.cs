//CED001


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
			
			//Version 1.1 Filter can handle both Edition and EditionNumberResolved, can be part of a multi-element component part
			//Version 1.0 Filter handles Edition only (no EditionNumberResolved)
			
			handled = false;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null) return null;
			if (citation == null || citation.Reference == null) return null;
			if (componentPart.Scope == ComponentPartScope.ParentReference && citation.Reference.ParentReference == null) return null;
			
			
			var editionFieldElement = componentPart.GetFieldElements().FirstOrDefault<FieldElement>(item => item.PropertyId == ReferencePropertyId.Edition || item.PropertyId == ReferencePropertyId.EditionNumberResolved);
			if (editionFieldElement == null) return null;

		
			string editionNumberResolved;
			if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				editionNumberResolved = citation.Reference.ParentReference.EditionNumberResolved;
			}
			else
			{
				editionNumberResolved = citation.Reference.EditionNumberResolved;
			}
			
			
			//now suppress the output ONLY if edition number is 1
			if (editionNumberResolved == "1")
			{
				componentPart.Elements.Remove(editionFieldElement);
			}
			
			return null;
		}
		
	
		
		
		//FYI
		/*
		public string EditionNumberResolved
		{
			get
			{
				if (string.IsNullOrEmpty(_valueData.Edition))
				{
					return string.Empty;
				}

				return new ReferenceEditionNumberRegex().Match(_valueData.Edition).Value;
				//ReferenceEditionNumberRegex -> 	\d+
			}
		}
		*/
	}
}