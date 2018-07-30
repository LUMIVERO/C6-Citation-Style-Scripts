//TRE003

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
			
			Reference thisReference = citation.Reference;
			if (thisReference == null) return false;
			
			Reference thisParentReference = thisReference.ParentReference;
			if (thisParentReference == null) return false;
			
			List<Person> thisAuthors = thisReference.AuthorsOrEditorsOrOrganizations.ToList();
			if (thisAuthors == null || !thisAuthors.Any()) return false;
			
			CitationManager cm = citation.CitationManager;
			if (cm == null) return false;
			
			foreach(Citation otherCitation in cm.BibliographyCitations)
			{
				if (otherCitation == null) continue;
				
				Reference otherReference = otherCitation.Reference;
				if (otherReference == null) continue;
				
				if (otherReference == thisReference) continue;
				
				Reference otherParentReference = otherReference.ParentReference;
				if (otherParentReference == null) continue;
				if (otherParentReference != thisParentReference) continue;
				
				List<Person> otherAuthors = otherReference.AuthorsOrEditorsOrOrganizations.ToList();
				if (otherAuthors == null || !otherAuthors.Any()) continue;
				
				if (thisAuthors.SequenceEqual(otherAuthors)) return true;
			}
			
			//still here? so other contribution by same other from same reference found
			return false;
		}
	}
}