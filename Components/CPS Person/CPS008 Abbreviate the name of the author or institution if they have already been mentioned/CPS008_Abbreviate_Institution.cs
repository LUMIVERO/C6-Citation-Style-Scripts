// CPS 008
//Version: 2.6
//Abbreviate organization that has been mentioned before

using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Citations;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;

namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
		:
		IComponentPartFilter
	{
		//Version 2.6	Fix bug that text before and after abbreviation is changed for all organisations in an authoring group
		//Version 2.5	If only the abbreviation of an organisation is to be shown, any text before and/or after is suppressed
        //Version 2.4   If Citavi demands that output is suppressed (e.g. when collapsing multiple citations of same author), this script will terminate
		//Version 2.3	Uses OnBeforeFormatOrganization to switch organization name output ot abbreviation only
		//Version 2.2	Uses PersonFormatter of PersonFieldElement
		//Version 2.1 	Confine repetition detection to organizational names only, so that ambiguity of individual persons' names can be handled correctly again by Citavi
		//				Therefore, name of helper method was changed from  GetPreviouslyMentionedPersons to GetPreviouslyMentionedOrganizations
		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			handled = false;

			if (citation == null || citation.Reference == null) return null;
			if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			
			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return null;
			if (placeholderCitation.YearOnly) return null;
			
			PersonFieldElement personFieldElement = componentPart.Elements[0] as PersonFieldElement;
			if (personFieldElement == null) return null;
            if (personFieldElement.SuppressOutput) return null;

			IEnumerable<Person> persons = personFieldElement.GetPersonsCited(citation);
			if (persons == null || !persons.Any()) return null;
			
			if (!persons.Any(p => p.IsOrganization)) return null;

			IEnumerable<Person> previouslyMentionedOrganizations = GetDistinctPreviouslyMentionedOrganizations(citation);
			if (previouslyMentionedOrganizations == null || !previouslyMentionedOrganizations.Any()) return null;
			
			var textBeforeAbbreviation = personFieldElement.OrganizationTextBeforeAbbreviation.Text;
			var textAfterAbbreviation = personFieldElement.OrganizationTextAfterAbbreviation.Text;
			
			
			BeforeFormatOrganizationEventArgs b;
			personFieldElement.PersonFormatter.BeforeFormatOrganization += 
			(sender, e) =>
			{
				b = (BeforeFormatOrganizationEventArgs)e;
				if (b.Organization == null) return;
				
				if (previouslyMentionedOrganizations.Contains(b.Organization))
				{
					b.NameOrder = OrganizationNameOrder.AbbreviationOnly;
					b.TextBeforeAbbreviation = null;
					b.TextAfterAbbreviation = null;
				}
				else
				{
					b.NameOrder = personFieldElement.OrganizationNameOrder;
					b.TextBeforeAbbreviation = textBeforeAbbreviation;
					b.TextAfterAbbreviation = textAfterAbbreviation;
				}
			};
			
			return null;
		}
		
		private HashSet<Person> GetDistinctPreviouslyMentionedOrganizations(Citation citation)
		{
			if (citation == null) return null;
			
			HashSet<Person> organizationSet = new HashSet<Person>();

			Citation previousCitation = citation.PreviousPrintingCitation;
			while (previousCitation != null)
			{
				Reference previousReference = previousCitation.Reference;
				if (previousReference != null)
				{
					IEnumerable<Person> previousPersons = previousReference.AuthorsOrEditorsOrOrganizations.AsEnumerable<Person>();
					if (previousPersons != null)
					{
						foreach (Person person in previousPersons)
						{
							if (person != null && person.IsOrganization) 
							{
								organizationSet.Add(person);
							}
						}
					}
				}
				
				previousCitation = previousCitation.PreviousPrintingCitation;
			}
			
			return organizationSet;
		}
	}
}
