//TRE007 Article from THIS journal has been cited before
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
			if (citation.Reference.Periodical == null) return false;
			
			
			FootnoteCitation currentFootnoteCitation = citation as FootnoteCitation;
			if (currentFootnoteCitation != null)
			{
				if (currentFootnoteCitation.RuleSetOverride != RuleSetOverride.None) return false;
				if (currentFootnoteCitation.YearOnly || currentFootnoteCitation.PersonOnly) return false;
				
				foreach (FootnoteCitation otherFootnoteCitation in citation.CitationManager.FootnoteCitations)
				{
					if (otherFootnoteCitation == currentFootnoteCitation) break;
					if (otherFootnoteCitation.Reference == null) continue;
					if (otherFootnoteCitation.Reference.Periodical == null) continue;
					if (otherFootnoteCitation.Reference.Periodical.Equals(currentFootnoteCitation.Reference.Periodical)) return true;
				}
				
				//still here? 
				return false;
			}
			
			
			InTextCitation currentInTextCitation = citation as InTextCitation;
			if (currentInTextCitation != null)
			{
				if (currentInTextCitation.RuleSetOverride != RuleSetOverride.None) return false;
				if (currentInTextCitation.YearOnly || currentInTextCitation.PersonOnly) return false;
				
				foreach (InTextCitation otherInTextCitation in citation.CitationManager.InTextCitations)
				{
					if (otherInTextCitation == currentInTextCitation) break;
					if (otherInTextCitation.Reference == null) continue;
					if (otherInTextCitation.Reference.Periodical == null) continue;
					if (otherInTextCitation.Reference.Periodical.Equals(currentInTextCitation.Reference.Periodical)) return true;					
				}
				
				//still here?
				return false;
			}
			
			
			BibliographyCitation currentBibliographyCitation = citation as BibliographyCitation;
			if (currentBibliographyCitation != null)
			{
				foreach(BibliographyCitation otherBibliographyCitation in citation.CitationManager.BibliographyCitations)
				{
					if (otherBibliographyCitation == currentBibliographyCitation) break;
					if (otherBibliographyCitation.Reference == null) continue;
					if (otherBibliographyCitation.Reference.Periodical == null) continue;
					if (otherBibliographyCitation.Reference.Periodical.Equals(currentBibliographyCitation.Reference.Periodical)) return true;
				}
				
				//still here
				return false;
			}
			
			return false;
		}
	}
}
