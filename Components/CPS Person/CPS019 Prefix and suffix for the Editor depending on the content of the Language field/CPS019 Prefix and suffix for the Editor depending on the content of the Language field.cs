//C6#CPS019
//C5#43124
//Description: Different prefix or suffix depending on language of reference
//Version: 1.2: Can be used in Editors AND AuthorsOrEditorsOrOrganizations components now
//Version: 1.1: Slight improvements

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
			if (componentPart == null) return null;
			if (!componentPart.Elements.Any()) return null;
			
			Reference reference = citation.Reference;
			if (reference == null) return null;
			
			Reference parentReference = reference.ParentReference;
			
			string language = string.Empty;
			if (componentPart.Scope == ComponentPartScope.Reference)
			{
				language = reference.Language.ToUpperInvariant();
			}
			else if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				if (parentReference == null) return null;
				language = parentReference.Language.ToUpperInvariant();				
			}
			
			if (string.IsNullOrEmpty(language)) return null;

			PersonFieldElement authorsOrEditorsOrOrganizationsFieldElement = componentPart.Elements.OfType<PersonFieldElement>().Where(field => field.PropertyId == ReferencePropertyId.Editors || field.PropertyId == ReferencePropertyId.AuthorsOrEditorsOrOrganizations).FirstOrDefault() as PersonFieldElement;
			if (authorsOrEditorsOrOrganizationsFieldElement == null) return null;
			
			if (componentPart.Scope == ComponentPartScope.Reference)
			{
				if
				(
					citation.Reference.AuthorsOrEditorsOrOrganizationsPersonRoleResolved != PersonRole.Editor &&
					citation.Reference.AuthorsOrEditorsOrOrganizationsPersonRoleResolved != PersonRole.Organization
				) return null;
			}
			else if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				if
				(
				citation.Reference.ParentReference.AuthorsOrEditorsOrOrganizationsPersonRoleResolved != PersonRole.Editor &&
				citation.Reference.ParentReference.AuthorsOrEditorsOrOrganizationsPersonRoleResolved != PersonRole.Organization
				) return null;
			}
			
			#region German
			
			if (language.Contains("DE"))
			{
				authorsOrEditorsOrOrganizationsFieldElement.GroupPrefixSingular.Text = "";
				authorsOrEditorsOrOrganizationsFieldElement.GroupPrefixPlural.Text = "";
				
				authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixSingular.Text = " (Hrsg.)";
				authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixPlural.Text = " (Hrsg.)";
				
				return null;
			}
			
			#endregion 
			
			#region English
			
			if (language.Contains("EN"))
			{
				authorsOrEditorsOrOrganizationsFieldElement.GroupPrefixSingular.Text = "";
				authorsOrEditorsOrOrganizationsFieldElement.GroupPrefixPlural.Text = "";
				
				authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixSingular.Text = " (ed.)";
				authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixPlural.Text = " (eds.)";
				
				return null;
			}
			
			#endregion 
			
			#region French
			
			if (language.Contains("FR"))
			{
				authorsOrEditorsOrOrganizationsFieldElement.GroupPrefixSingular.Text = "";
				authorsOrEditorsOrOrganizationsFieldElement.GroupPrefixPlural.Text = "";
				
				authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixSingular.Text = " (éd.)";
				authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixPlural.Text = " (éds.)";
				
				return null;
			}
			
			#endregion 
			
			#region Italian
			
			if (language.Contains("IT"))
			{
				authorsOrEditorsOrOrganizationsFieldElement.GroupPrefixSingular.Text = "";
				authorsOrEditorsOrOrganizationsFieldElement.GroupPrefixPlural.Text = "";
				
				authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixSingular.Text = " (ed.)";
				authorsOrEditorsOrOrganizationsFieldElement.GroupSuffixPlural.Text = " (eds.)";
				
				return null;
			}
			
			#endregion 
		
			return null;
		}
	}
}
