//#C6_TRE002
//#C5_43244
//Version 2.1

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
		//At least one other reference of same author(s)/editor(s)/organization(s) also cited
		//Version 2.1 NoBib citation are ignored
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			var testEqualityBy = PersonIdentityTest.ByInternalID; //adjust here to your needs
																//e.g. PersonIdentityTest.ByLastNameFirstName
                                                                //e.g. PersonIdentityText.ByFullName
																//e.g. PersonIdentityTest.ByInternalID <- recommended for most cases
			
			if (citation == null) return false;

			CitationManager citationManager = citation.CitationManager;
			if (citationManager == null) return false;

			Reference currentReference = citation.Reference;
			if (currentReference == null) return false;

			IEnumerable<Person> currentPersons = currentReference.AuthorsOrEditorsOrOrganizations;
			if (currentPersons == null || currentPersons.Count() == 0) return false;
			
			
			BibliographyCitation currentBibliographyCitation = citation as BibliographyCitation;
			if (currentBibliographyCitation == null)
			{
				PlaceholderCitation  currentPlaceholderCitation = citation as PlaceholderCitation;
				if (currentPlaceholderCitation == null) return false;
				
				currentBibliographyCitation = currentPlaceholderCitation.CorrespondingBibliographyCitation;
			}
			if (currentBibliographyCitation == null) return false;
			if (currentBibliographyCitation.NoBib.GetValueOrDefault(false)) return false;
			
			
			
			IEnumerable<string> currentIdentifiers = GetPersonIdentifiers(currentPersons, testEqualityBy);

			foreach (BibliographyCitation otherBibliographyCitation in citationManager.BibliographyCitations)
			{
				if (otherBibliographyCitation == null) continue;
				if (otherBibliographyCitation == currentBibliographyCitation) continue;
				if (otherBibliographyCitation.NoBib.GetValueOrDefault(false)) continue;
				
				var otherReference = otherBibliographyCitation.Reference;
				if (otherReference == null) continue;
				if (otherReference == currentReference) continue;

				var otherPersons = otherReference.AuthorsOrEditorsOrOrganizations;
				if (otherPersons == null || otherPersons.Count() == 0) continue;

				
				var otherIdentifiers = GetPersonIdentifiers(otherPersons, testEqualityBy);
				
				if (testEqualityBy == PersonIdentityTest.ByInternalID)
				{
					//object identity
					if (otherPersons.SequenceEqual(currentPersons)) return true;
				}
				else
				{
					if (otherIdentifiers.SequenceEqual(currentIdentifiers)) return true;
				}
			}

			return false;
		}
		
		public enum PersonIdentityTest
        {
			ByInternalID,
            ByLastName,
            ByLastNameFirstName,
            ByFullName
        }
		
		public IEnumerable<string> GetPersonIdentifiers(IEnumerable<Person> persons, PersonIdentityTest method)
		{
			if (persons == null || !persons.Any()) return Enumerable.Empty<string>();
			
			switch (method)
			{
				case PersonIdentityTest.ByInternalID:
				default:
					//return persons.Select(person => person.Id);
					return Enumerable.Empty<string>(); //test for object identity instead
					break;
					
				case PersonIdentityTest.ByFullName:
					return persons.Select(person => person.FullName);
					break;
					
				case PersonIdentityTest.ByLastName:
					return persons.Select(person => person.LastName);
					break;
					
				case PersonIdentityTest.ByLastNameFirstName:
					return persons.Select(person => person.LastName + person.FirstName);
					break;
			}
		}
	}
}
