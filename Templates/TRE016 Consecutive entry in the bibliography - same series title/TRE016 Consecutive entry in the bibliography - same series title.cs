//TRE016 - Consecutive entry in the bibliography - same series title
//Description:	Previous bibliography citation has the same series title as THIS bibliography citation
//Version 1.0

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
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;
			if (citation.CitationManager == null) return false;
			if (citation.Reference == null) return false;
			if (citation.Reference.SeriesTitle == null) return false;
			
			//filter deactivates itself, if the bibliography is NOT YET completely sorted
			//this is necessary to avoid that this filter in turn changes the sort order, that it depends upon
			if (citation.CitationManager.BibliographyCitations.IsSorted == false) return false;

			#region ThisBibliographyCitation

			var thisBibliographyCitation = citation as BibliographyCitation;
			if (thisBibliographyCitation == null) return false;

			#endregion ThisBibliographyCitation

			#region PreviousBibliographyCitation

			var previousBibliographyCitation = GetPreviousVisibleBibliographyCitation(thisBibliographyCitation);
			if (previousBibliographyCitation == null) return false;
			if (previousBibliographyCitation.Reference == null) return false;
			if (previousBibliographyCitation.NoBib == true) return false;

			#endregion PreviousBibliographyCitation

			if (previousBibliographyCitation == thisBibliographyCitation) return false;
			if (previousBibliographyCitation.Reference.SeriesTitle == null) return false;
			if (previousBibliographyCitation.Reference.SeriesTitle.Equals(thisBibliographyCitation.Reference.SeriesTitle)) return true;

			//still here
			return false;
		}
		
		#region GetPreviousVisibleCitation

		private static BibliographyCitation GetPreviousVisibleBibliographyCitation(BibliographyCitation bibliographyCitation)
		{
			if (bibliographyCitation == null) return null;
			BibliographyCitation previousBibliographyCitation = bibliographyCitation;

			//consider nobib
			do
			{
				previousBibliographyCitation = previousBibliographyCitation.PreviousBibliographyCitation;
				if (previousBibliographyCitation == null) return null;

			} while (previousBibliographyCitation.NoBib == true);

			//still here? found one!
			return previousBibliographyCitation;
		}

		#endregion GetPreviousCitation
	}
}
