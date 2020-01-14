//TXY018
//Field Edition (EditionNumberResolved) is exactly 1 (considers a parent reference's edition field if applicable)

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
			
			
			string editionNumberResolved;
		
			if (reference.HasCoreField(ReferenceTypeCoreFieldId.Edition))
			{
				editionNumberResolved = citation.Reference.EditionNumberResolved;
			}
			else if (parentReference != null && parentReference.HasCoreField(ReferenceTypeCoreFieldId.Edition))
			{
				editionNumberResolved = citation.Reference.ParentReference.EditionNumberResolved;
			}
			else
			{
				return false;
			}
			
			//ONLY if edition number is 1
			if (editionNumberResolved == "1")
			{
				return true;
			}
			
			return false;

		}
   }
}
