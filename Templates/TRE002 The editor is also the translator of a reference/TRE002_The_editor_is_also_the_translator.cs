using System;
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
		//Editors == Translators (applicable for collected works/edited books etc. AND contributions therein)
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;
			if (citation.Reference == null) return false;
			
			Reference currentReference = citation.Reference;
			Reference parentReference = currentReference.ParentReference;
			
			bool currentReferenceHasEditors = 
				currentReference.HasCoreField(ReferenceTypeCoreFieldId.Editors) && 
				currentReference.Editors != null && 
				currentReference.Editors.Any();
			
			bool currentReferenceHasTranslators = 
				currentReference.Translators != null && 
				currentReference.Translators.Any();
			
			bool parentReferenceHasEditors = 
				parentReference != null &&
				parentReference.HasCoreField(ReferenceTypeCoreFieldId.Editors) &&
				parentReference.Editors != null &&
				parentReference.Editors.Any();
			
			bool parentReferenceHasTranslators = 
				parentReference != null &&
				parentReference.Translators != null &&
				parentReference.Translators.Any();
			
			
			if (currentReferenceHasTranslators && currentReferenceHasEditors)
			{
				return CollectionUtility.ContentEquals(currentReference.Translators, currentReference.Editors, false);
			}
			
			if (currentReferenceHasTranslators && parentReferenceHasEditors)
			{
				return CollectionUtility.ContentEquals(currentReference.Translators, parentReference.Editors, false);
			}
			
			if (parentReferenceHasTranslators && parentReferenceHasEditors)
			{
				return CollectionUtility.ContentEquals(parentReference.Translators, parentReference.Editors, false);
			}
			
			
			//still here? then condition is not fulfilled
			return false;
		}
	}
}
