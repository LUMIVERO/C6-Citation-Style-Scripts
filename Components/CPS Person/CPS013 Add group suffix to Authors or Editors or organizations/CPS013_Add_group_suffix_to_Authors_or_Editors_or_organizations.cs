//#43125
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
			
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			
			PersonFieldElement authorsOrEditorsOrOrganizationsFieldElement = componentPart.Elements.OfType<PersonFieldElement>()
				.Where(element => element.PropertyId == ReferencePropertyId.AuthorsOrEditorsOrOrganizations)
				.FirstOrDefault() as PersonFieldElement;
			
			if (authorsOrEditorsOrOrganizationsFieldElement == null) return null;
			if 
			(
				citation.Reference.AuthorsOrEditorsOrOrganizationsPersonRoleResolved != PersonRole.Editor &&
				citation.Reference.AuthorsOrEditorsOrOrganizationsPersonRoleResolved != PersonRole.Organization
			) return null;
			
			authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixSingular.Text = " (Hrsg.)";
			//authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixSingular.FontStyle = FontStyle.Bold;
			
			authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixPlural.Text = " (Hrsg.)";
			//authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixPlural.FontStyle = FontStyle.Bold;
			
			handled = false;
			return null;
		}
	}
}