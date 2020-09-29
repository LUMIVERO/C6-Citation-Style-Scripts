//TNE002
//Edition is NOT empty (considers a parent reference's edition field if applicable)

using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;

namespace SwissAcademic.Citavi.Citations
{
	public class CustomTemplateCondition
		:
		ITemplateConditionMacro
	{
	
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{

			if (citation == null) return false;
			
			var reference = citation.Reference;
			if (reference == null) return false;
			
			var parentReference = citation.Reference.ParentReference;
			
			if (reference.HasCoreField(ReferenceTypeCoreFieldId.Edition))
			{
				return !string.IsNullOrWhiteSpace(reference.Edition);
			}
			else if (parentReference != null && parentReference.HasCoreField(ReferenceTypeCoreFieldId.Edition))
			{
				return !string.IsNullOrWhiteSpace(parentReference.Edition);
			}
			else
			{
				return false;
			}
		}
	}
}
