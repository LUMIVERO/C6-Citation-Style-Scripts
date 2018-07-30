//C6#COT011
//C5#431520
//Description: Using o. O. for missing publication place
//Version: 1.1
//Added consideration of parent reference. Preferred reference is that "in scope" of component part. If that does not have a place of publication
//but its parent reference does, then the place of publication of the latter is considered. 

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
			handled = false;

			if (citation == null) return null;

            Reference currentReference = citation.Reference;
            if (currentReference == null) return null;
			
			Reference currentParentReference = currentReference.ParentReference; // can be null;

            if (componentPart == null) return null;
            if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;

            Reference referenceInScope = null;
            if (componentPart.Scope == ComponentPartScope.ParentReference && currentParentReference != null)
            {
                referenceInScope = currentParentReference;
            }
            else if (componentPart.Scope == ComponentPartScope.Reference)
            {
                referenceInScope = currentReference;
            }
            
			var firstPlaceOfPublicationFieldElement = componentPart.Elements.OfType<PlaceOfPublicationFieldElement>().FirstOrDefault() as PlaceOfPublicationFieldElement;
			if (firstPlaceOfPublicationFieldElement == null) return null;

			string placeOfPublicationString = referenceInScope.GetValue(firstPlaceOfPublicationFieldElement.PropertyId) as string;
			if (!string.IsNullOrEmpty(placeOfPublicationString)) return null;	//place of publication will be output automatically by Citavi
			
			
			//if referenceInScope does not have a place of publication and there is still a parentReference then try that one
			if (currentParentReference != null) placeOfPublicationString = currentParentReference.GetValue(firstPlaceOfPublicationFieldElement.PropertyId) as string;

			var outputString = "o. O.";
			if (!string.IsNullOrEmpty(placeOfPublicationString)) outputString = placeOfPublicationString;
			
			var outputLiteralElement = new LiteralElement(componentPart, outputString);
			outputLiteralElement.FontStyle = firstPlaceOfPublicationFieldElement.FontStyle;
			componentPart.Elements.ReplaceItem(firstPlaceOfPublicationFieldElement, outputLiteralElement);


			//all literal elements should always be output:
			foreach (LiteralElement literalElement in componentPart.Elements.OfType<LiteralElement>())
			{
				literalElement.ApplyCondition = ElementApplyCondition.Always;
			}

			return null;
		}
	}
}