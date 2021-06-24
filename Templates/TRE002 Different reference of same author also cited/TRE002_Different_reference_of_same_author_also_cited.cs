//TRE002
//Version 1.0

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
		//Other reference of same author(s)/editor(s)/organization(s) cited BEFORE
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			var testEqualityBy = PersonIdentityTest.ByInternalID;	//adjust here to your needs
																	//e.g. PersonIdentityTest.ByLastNameFirstName
																	//e.g. PersonIdentityText.ByFullName
																	//e.g. PersonIdentityTest.ByInternalID <- recommended for most cases

			bool considerNoBibInBibliographyCitations = false;
			bool considerNoBibInInTextCitations = false;
			bool considerNoBibInFootnoteCitations = false;

			if (citation == null) return false;

			CitationManager citationManager = citation.CitationManager;
			if (citationManager == null) return false;

			Reference currentReference = citation.Reference;
			if (currentReference == null) return false;

			IEnumerable<Person> currentPersons = currentReference.AuthorsOrEditorsOrOrganizations;
			if (currentPersons == null || currentPersons.Count() == 0) return false;

			IEnumerable<PublicationCitation> allCitations = null;

			PublicationCitation currentPublicationCitation = citation as PublicationCitation;
			if (currentPublicationCitation == null) return false;

			#region InTextCitations

			InTextCitation currentInTextCitation = citation as InTextCitation;
			if (currentInTextCitation != null)
			{
				if (currentInTextCitation.BibOnly) return false;
				allCitations = considerNoBibInInTextCitations ? 
					citationManager.InTextCitations.Where(item => !item.BibOnly).Cast<PublicationCitation>() :
					citationManager.InTextCitations.Where(item => !item.BibOnly && item.CorrespondingBibliographyCitation != null && !item.CorrespondingBibliographyCitation.NoBib.GetValueOrDefault(false));
			} 

			#endregion 

			#region FootnoteCitations

			if (allCitations == null)
			{
				FootnoteCitation currentFootnoteCitation = citation as FootnoteCitation;
				if (currentFootnoteCitation != null)
				{
					if (currentFootnoteCitation.BibOnly) return false;
					allCitations = considerNoBibInFootnoteCitations ?
						citationManager.FootnoteCitations.Where(item => !item.BibOnly).Cast<PublicationCitation>() :
						citationManager.FootnoteCitations.Where(item => !item.BibOnly && item.CorrespondingBibliographyCitation != null && !item.CorrespondingBibliographyCitation.NoBib.GetValueOrDefault(false));
				}
			}

			#endregion

			#region BibliographyCitations

			if (allCitations == null)
			{
				BibliographyCitation currentBibliographyCitation = citation as BibliographyCitation;
				if (currentBibliographyCitation.NoBib.GetValueOrDefault(false)) return false;
				if (currentBibliographyCitation != null)
				{
					allCitations = citationManager.BibliographyCitations.Where(item => !item.NoBib.GetValueOrDefault(false)).Cast<PublicationCitation>();
				}
			}

			#endregion 

			IEnumerable<string> currentIdentifiers = GetPersonIdentifiers(currentPersons, testEqualityBy);

			foreach (PublicationCitation otherPublicationCitation in allCitations)
			{
				if (otherPublicationCitation == null) continue;
				
				if (otherPublicationCitation == currentPublicationCitation) break;
				
				var otherReference = otherPublicationCitation.Reference;
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
