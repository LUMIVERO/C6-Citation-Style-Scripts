//C6#CPS025
//C5#43117
//Description:	Removes brackets on Organization name if abbreviation is missing
//Version 3.1:	Simplifies reference to person field element
//Version 3.0:	Takes the ComponentPartScope into account

//IMPORTANT:	Requires Citavi 5 or Citavi 6

//How does the correction of brackets on Institution names work?
//WRONG:		UN (United Nations) becomes (United Nations) if there is no abbreviation
//CORRECT:		United Nations
//This is corrected by this FilterCode.

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
			if (citation.Reference == null) return null;

			if (componentPart == null) return null;

			Reference referenceResolved = null;
			if (componentPart.Scope == ComponentPartScope.ParentReference && citation.Reference.ParentReference != null) referenceResolved = citation.Reference.ParentReference;
			else referenceResolved = citation.Reference;
			if (referenceResolved == null) return null;

			var organizationsFieldElement = componentPart.Elements.OfType<PersonFieldElement>().FirstOrDefault() as PersonFieldElement;
			if (organizationsFieldElement == null) return null;

			IList<Person> personsAndOrganizations = referenceResolved.GetValue(organizationsFieldElement.PropertyId) as IList<Person>;
			if (personsAndOrganizations == null || personsAndOrganizations.Count == 0) return null;

			if (organizationsFieldElement.OrganizationNameOrder == OrganizationNameOrder.AbbreviationName)
			{
				//"UN [United Nations]"
				organizationsFieldElement.OrganizationTextBeforeNameApplyCondition = TextBeforeAfterApplyCondition.AllAttributesHaveData;
				organizationsFieldElement.OrganizationTextAfterNameApplyCondition = TextBeforeAfterApplyCondition.AllAttributesHaveData;
			}
			else if (organizationsFieldElement.OrganizationNameOrder == OrganizationNameOrder.NameAbbreviation)
			{
				//"United Nations [UN]"
				organizationsFieldElement.OrganizationTextBeforeAbbreviationApplyCondition = TextBeforeAfterApplyCondition.AllAttributesHaveData;
				organizationsFieldElement.OrganizationTextAfterAbbreviationApplyCondition = TextBeforeAfterApplyCondition.AllAttributesHaveData;
			}
			return null;

		}
	}
}
