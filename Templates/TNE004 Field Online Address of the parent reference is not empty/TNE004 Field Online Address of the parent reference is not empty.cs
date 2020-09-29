//TNE006
//If the Online Address field of a contribution is empty, check whether the parent reference's Online Address field is NOT empty

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
			if (citation.Reference.ParentReference == null) return false;
			
			var parentReference = citation.Reference.ParentReference;
			
			if (parentReference != null && parentReference.HasCoreField(ReferenceTypeCoreFieldId.OnlineAddress) && 
				reference.HasCoreField(ReferenceTypeCoreFieldId.OnlineAddress) && string.IsNullOrWhiteSpace(reference.OnlineAddress))
			{
				return !string.IsNullOrWhiteSpace(parentReference.OnlineAddress);
			}
			else
			{
				return false;
			}
		}
	}
}
