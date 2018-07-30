//C6#COT003
//C5#431611
//Description: Suppress cross-ref to 1st footnote index if 1st occurance is inside same footnote
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
			if (citation.Reference == null) return null;

			FootnoteCitation thisFootnoteCitation = citation as FootnoteCitation;
			if (thisFootnoteCitation == null) return null;

			if (componentPart == null) return null;

			Reference referenceToLookFor = null;
			if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				referenceToLookFor = citation.Reference.ParentReference;
			}
			else
			{
				referenceToLookFor = citation.Reference;
			}
			if (referenceToLookFor == null) return null;


			//iterate through all previous footnote citations to find first occurance of referenceToLookFor
			var citationManager = thisFootnoteCitation.CitationManager;
			if (citationManager == null) return null;


			FootnoteCitation firstFootnoteCitationToLookFor = null;
			foreach (FootnoteCitation previousFootnoteCitation in citationManager.FootnoteCitations)
			{
				if (previousFootnoteCitation == null) continue;
				if (previousFootnoteCitation == thisFootnoteCitation) break;

				if (previousFootnoteCitation.Reference == null) continue;
				if (previousFootnoteCitation.Reference == referenceToLookFor)
				{
					firstFootnoteCitationToLookFor = previousFootnoteCitation;
					break;
				}
			}

			if (firstFootnoteCitationToLookFor == null)
			{
				handled = true;
				return null;		//no first footnote index, NO output
			}

			//Still here? First footnote index was found.
			//caution: this does not work: firstFootnoteCitationToLookFor.FootnoteIndex == thisFootnoteCitation.FootnoteIndex
			if (firstFootnoteCitationToLookFor.FootnoteIndex.Equals(thisFootnoteCitation.FootnoteIndex))
			{
				handled = true;
				return null;		//same footnote, so NO output
			}
			else
			{
				handled = false;
				return null;
			}
		}
	}
}