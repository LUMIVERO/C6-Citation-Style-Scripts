//C6#CPS019
//C5#43124
//Description: Different prefix or suffix depending on language of reference
//Version: 1.0

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
			
			IReference reference = citation.Reference;
			if (reference == null) return null;
			
			IReference parentReference = reference.ParentReference;
			
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

			PersonFieldElement editorFieldElement = componentPart.Elements.OfType<PersonFieldElement>().Where(field => field.PropertyId == ReferencePropertyId.Editors).FirstOrDefault() as PersonFieldElement;
			if (editorFieldElement == null) return null;
				
			#region English
			
			if (language.Contains("EN"))
			{
				editorFieldElement.GroupPrefixSingular.Text = "ed. by ";
				editorFieldElement.GroupPrefixPlural.Text = "ed. by ";
				
				editorFieldElement.GroupSuffixSingular.Text = "";
				editorFieldElement.GroupSuffixPlural.Text = "";
				
				return null;
			}
			
			#endregion 
			
			#region French
			
			if (language.Contains("FR"))
			{
				editorFieldElement.GroupPrefixSingular.Text = "publ. par ";
				editorFieldElement.GroupPrefixPlural.Text = "publ. par ";
				
				editorFieldElement.GroupSuffixSingular.Text = "";
				editorFieldElement.GroupSuffixPlural.Text = "";
				
				return null;
			}
			
			#endregion 
			
			#region Italian
			
			if (language.Contains("IT"))
			{
				editorFieldElement.GroupPrefixSingular.Text = "a cura di ";
				editorFieldElement.GroupPrefixPlural.Text = "a cura di ";
				
				editorFieldElement.GroupSuffixSingular.Text = "";
				editorFieldElement.GroupSuffixPlural.Text = "";
				
				return null;
			}
			
			#endregion 
		
			return null;
		}
	}
}