//C6#COT041
//Description:	Replace Space by Non-breaking Space in the Series Title
//Version 1.1:	Added consideration of parent reference. Preferred reference is that "in scope" of component part.
//Version 1.0

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
			
			if (componentPart == null) return null;
			
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
			
			SeriesTitle seriesTitle = referenceInScope.SeriesTitle;
			if (seriesTitle == null) return null;	

			SeriesTitleFieldElement seriesTitleFieldElement = componentPart.Elements.OfType<SeriesTitleFieldElement>().FirstOrDefault() as SeriesTitleFieldElement;
			if (seriesTitleFieldElement == null) return null;

			bool found = false;
			TextUnitCollection textUnits = seriesTitleFieldElement.GetTextUnits(citation, template);
			if (textUnits == null) return null;

			foreach(ITextUnit textUnit in textUnits)
			{
				if (textUnit.Text.Contains(" "))
				{
					found = true;
					textUnit.Text = textUnit.Text.Replace(" ", "\u00A0");
				}
			}
			
			if (found)
			{
				componentPart.Elements.ReplaceItem(seriesTitleFieldElement, textUnits.TextUnitsToLiteralElements(componentPart));
			}
			
			return null;
		}
	}
}
