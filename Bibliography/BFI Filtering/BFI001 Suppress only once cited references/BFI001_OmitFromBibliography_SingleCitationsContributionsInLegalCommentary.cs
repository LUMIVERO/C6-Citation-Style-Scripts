//BFI001
//21.06.2018 
//Suppress "Contribution in legal commentary", Statute, Court decision in reference list
//Suppress any references that are only cited once 

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Citations;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;

namespace SwissAcademic.Citavi.Comparers
{
	public class CustomCitationComparer
		:
		ICustomCitationComparerMacro
	{
		public int Compare(Citation x, Citation y)
		{
			#region Preliminary Tests 

			//First we make sure we are comparing BibliographyCitations only
			var xBibliographyCitation = x as BibliographyCitation;
			var yBibliographyCitation = y as BibliographyCitation;

			if (xBibliographyCitation == null || yBibliographyCitation == null) return 0;
			var xReference = xBibliographyCitation.Reference;
			var yReference = yBibliographyCitation.Reference;
			if (xReference == null || yReference == null) return 0;

			#endregion

			#region Define the default citation comparer for sorting

			var defaultCitationComparer = CitationComparer.AuthorYearTitleOrNoAuthorThenTitleYearAscending;

			#endregion

			#region Exclude certain reference types from bibliography

			//Set the NoBib flag to true for ContributionInLegalCommentary ("Beitrag im Gesetzeskommentar")
			if (xReference.ReferenceType == ReferenceType.ContributionInLegalCommentary) xBibliographyCitation.NoBib = true;
			if (yReference.ReferenceType == ReferenceType.ContributionInLegalCommentary) yBibliographyCitation.NoBib = true;

			//Set the NoBib flag to true for StatuteOrRegulation ("Gesetz / Verordnung")
			if (xReference.ReferenceType == ReferenceType.StatuteOrRegulation) xBibliographyCitation.NoBib = true;
			if (yReference.ReferenceType == ReferenceType.StatuteOrRegulation) yBibliographyCitation.NoBib = true;

			//Set the NoBib flag to true for CourtDecision ("Gerichtsentscheid")
			if (xReference.ReferenceType == ReferenceType.CourtDecision) xBibliographyCitation.NoBib = true;
			if (yReference.ReferenceType == ReferenceType.CourtDecision) yBibliographyCitation.NoBib = true;

			#endregion

			#region Exclude references that have only been cited once inside footnotes (in text citations are NOT counted)

			CitationManager citationManager = x.CitationManager;
			if (citationManager == null) return 0;
			int xCounter = 0;
			int yCounter = 0;
			foreach (FootnoteCitation fnCit in citationManager.FootnoteCitations)
			{
				if (fnCit.Reference == null) continue;
				if (fnCit.Reference == xReference) xCounter++;
				if (fnCit.Reference == yReference) yCounter++;
			}
			if (xCounter < 2) xBibliographyCitation.NoBib = true;
			if (yCounter < 2) yBibliographyCitation.NoBib = true;

			#endregion

			#region Apply default sorting

			return defaultCitationComparer.Compare(x, y);

			#endregion
		}
	}
}
