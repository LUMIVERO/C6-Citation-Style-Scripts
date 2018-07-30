//TRE005
//Collective volume has been cited before in bibliography or footnotes 
//(either the collective volume itself or any contribution from it)

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
			
			IReference currentReference = citation.Reference;
			if (currentReference == null) return false;
			
			IReference currentParentReference = currentReference.ParentReference;
			if (currentParentReference == null) return false;
			
			CitationManager citationManager = citation.CitationManager;
			if (citationManager == null) return false;
			
			#region Bibliography
			
			BibliographyCitation currentBibliographyCitation = citation as BibliographyCitation;
			if (currentBibliographyCitation != null)
			{
				if (!citationManager.BibliographyCitations.IsSorted) return false;
				foreach(BibliographyCitation otherBibliographyCitation in citationManager.BibliographyCitations)
				{
					if (otherBibliographyCitation == null) continue;
					if (otherBibliographyCitation == currentBibliographyCitation) break;
					
					IReference otherReference = otherBibliographyCitation.Reference;
					if (otherReference == null) continue;
					if (otherReference == currentReference) return true;
					if (otherReference == currentParentReference) return true;
					
					IReference otherParentReference = otherReference.ParentReference;
					if (otherParentReference == null) continue;
					if (otherParentReference == currentParentReference) return true;
					if (otherParentReference == currentReference) return true;
				}
				return false;
			}
			
			#endregion Bibliography
			
			#region Footnotes
			
			FootnoteCitation currentFootnoteCitation = citation as FootnoteCitation;
			if (currentFootnoteCitation != null)
			{
				foreach(FootnoteCitation otherFootnoteCitation in citationManager.FootnoteCitations)
				{
					if (otherFootnoteCitation == null) continue;
					if (otherFootnoteCitation == currentFootnoteCitation) break;
					
					IReference otherReference = otherFootnoteCitation.Reference;
					if (otherReference == null) continue;
					if (otherReference == currentReference) return true;
					if (otherReference == currentParentReference) return true;
					
					IReference otherParentReference = otherReference.ParentReference;
					if (otherParentReference == null) continue;
					if (otherParentReference == currentParentReference) return true;
					if (otherParentReference == currentReference) return true;
				}
				return false;
			}
			
			#endregion Footnotes
			
			//not implmented for InTextCitations
			return false;
		}
	}
}