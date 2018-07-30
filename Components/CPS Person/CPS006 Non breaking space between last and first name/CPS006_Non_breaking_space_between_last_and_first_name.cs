//C6#CPS006
//C5#43116
//Description: Non breaking space between last and first name
//Example: Meyers, J.°R. 
//Version: 1.1; added variables on top (see lines 14-17) to control breaking in and between prefix, last name, first and middle name(s)

using System.Linq;
using System.Collections.Generic;

namespace SwissAcademic.Citavi.Citations
{
	

	public class ComponentPartFilter
		:
		IComponentPartFilter
	{
		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			var useNonBreakingSpacesInAndBetweenFirstAndMiddleNames = true;		//if true, then e.g. Meyers, J.°R.
			var useNonBreakingSpaceBetweenLastAndFirstName = true;				//if true, then e.g. Meyers,°John Richard
			var useNonBreakingSpaceBetweenPrefixAndName = true;					//if true, then e.g. von°Bülow, V.
			var useNonBreakingHyphenInFirstAndMiddleName = true;				//if true, then e.g. Ewing, J.-R.
			
			handled = false;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;
			
			IEnumerable<PersonFieldElement> personFieldElements = componentPart.Elements.OfType<PersonFieldElement>();
			if (personFieldElements == null || personFieldElements.Count() == 0) return null;
			
			
			foreach(PersonFieldElement element in personFieldElements)
			{
				if (useNonBreakingSpacesInAndBetweenFirstAndMiddleNames)
				{
					element.FirstGroupUseNonBreakingSpaceInAndBetweenFirstAndMiddleNames = true;
					element.SecondGroupUseNonBreakingSpaceInAndBetweenFirstAndMiddleNames = true;
					element.LastPersonUseNonBreakingSpaceInAndBetweenFirstAndMiddleNames = true;				
				}

				if (useNonBreakingSpaceBetweenLastAndFirstName)
				{
					element.FirstGroupUseNonBreakingSpaceBetweenLastAndFirstName = true;
					element.SecondGroupUseNonBreakingSpaceBetweenLastAndFirstName = true;
					element.LastPersonUseNonBreakingSpaceBetweenLastAndFirstName = true;				
				}

				if (useNonBreakingSpaceBetweenPrefixAndName)
				{
					element.FirstGroupUseNonBreakingSpaceBetweenPrefixAndName = true;
					element.SecondGroupUseNonBreakingSpaceBetweenPrefixAndName = true;
					element.LastPersonUseNonBreakingSpaceBetweenPrefixAndName = true;
				}

				if (useNonBreakingHyphenInFirstAndMiddleName)
				{
					element.FirstGroupUseNonBreakingHyphenInFirstAndMiddleNames = true;
					element.SecondGroupUseNonBreakingHyphenInFirstAndMiddleNames = true;
					element.LastPersonUseNonBreakingHyphenInFirstAndMiddleNames = true;
				}
			}
			
			//Citavi will do the rest, no need to return any output
			return null;
		}
	}
}