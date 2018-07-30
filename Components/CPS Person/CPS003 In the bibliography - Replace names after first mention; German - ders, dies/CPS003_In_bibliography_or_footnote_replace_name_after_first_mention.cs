//C6#CPS003
//C5#43112
//Description: In bibliography or footnote replace name after first mention, ders, dies  
//Version: 1.17
#region Version History
//Version 1.17  Addes options for showing group prefix and/or group suffix (e.g.: "ders. (Hrsg)")
//Version 1.16  Added "dass." for Neutrum
//Version 1.15  Bugfix for deactivateFilterAcrossFootnotesIfPreviousCitationNotSolitair
//				New parameters "deactivateFilterForFirstInsideMultipleCitations" and "deactivateFilterForFirstAfterMultipleCitations"
//Version 1.14	Configurable output for missing author(s), e.g. DE: "o.A."
//Version 1.10	Improved GetPersonsDisplayed, renamed parameter deactivateFilterInOpCitIbidemSequence to deactivateFilterInIbidemIdemSequence and introduced
//				deactivateFilterAcrossFootnotesIfSeparatedByMoreThanOneIndexNo as well as deactivateFilterAcrossFootnotesIfPreviousCitationNotSolitair
//Version 1.9	Makeing V1.7 parameter configurable, deactivateFilterInOpCitIbidemSequence = false, and new parameter deactivateFilterAcrossFootnotes = true.
//Version 1.8	Bug-Correction: added forgotten case for "Organizations" and made retrieving persons of current and previous citation more flexible, based on component/template
//Version 1.7	New variable "outputInSmallCaps", "outputInBold", "outputUnderlined"
//				Component deactivates itself if the previous citation is from the same reference as the one before the previous citation (avoiding a sequence such as e.g.: [2] Ebd. -> [3] Ders.)
//				A group of several males is now "iid." instead of "eid."
//Version 1.6	GetPreviousVisibleCitation() method gets first previous citation where nobib = false or bibonly = false, 
//				Deactivate filter with /opt1 (see variable: deactivateFilterWithOption1Switch)
//Version 1.5	New variable "outputInItalics": output can quickly be changed between font style italics and neutral
//Version 1.4	New variable "deactivateFilterInsideMultipleCitations": inside multiple citations, filter can be switched off to allow for cite collapsing (see below)
//Version 1.3	Footnote index difference must not be > 0
//Version 1.2	Takes all combinations of number/sex into account
//Version 1.1	Takes organizations into account
#endregion Version History 

using System;
using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;

namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
		:
		IComponentPartFilter
	{



		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			//if the following is set to true a placeholder citation's output will not show ders./dies. although this would normally be the case
			//as e.g. the second one of the following two: {Meier 2010 #2} and {Meier 2010 #2:17-19 /opt1} 
			var deactivateFilterWithOption1Switch = true;								//default: true	

			//if the following is set to true, the citation collapsing can take place (if the style is set to omit author names)
			//true: 	(Mueller 2010, S. 10; 2011, S. 12f.; 2012, S. 17)o
			//false: 	(Mueller 2010, S. 10; ders. 2011, S. 12 f; ders. 2012, S. 17)
			var deactivateFilterInsideMultipleCitations = true;							//default: true
			var deactivateFilterForFirstInsideMultipleCitations = true;					//default: true; only applicable when deactivateFilterInsideMultipleCitations = false
			var deactivateFilterForFirstAfterMultipleCitations = true;					//default: true
			var deactivateFilterInIbidemIdemSequence = false;							//default: false
			var deactivateFilterAcrossFootnotes = false;								//default: false
			var deactivateFilterAcrossFootnotesIfSeparatedByMoreThanOneIndexNo = true; 	//default: true; only applicable when deactivateFilterAcrossFootnotes = false
			var deactivateFilterAcrossFootnotesIfPreviousCitationNotSolitair = true;	//default: true; ditto
			var outputInItalics = true;													//default: true
			var outputInSmallCaps = false;												//default: false
			var outputInBold = false;													//default: false
			var outputUnderlined = false;												//default: false

			var missingPersonsInfo = "o.A.";											//can be set to e.g. "o.A."/"ohne Autor"; leave empty otherwise
			var outputMissingPersonsInfoInItalics = false;								//default: false
			var outputMissingPersonsInfoInSmallCaps = false;							//default: false
			var outputMissingPersonsInfoInBold = false;									//default: false
			var outputMissingPersonsInfoUnderlined = false;								//default: false
			
			var showGroupPrefixIfPresent = false;										//default: false
			var showGroupSuffixIfPresent = true;										//default: true

			handled = false;
			var thisCitationIsPartOfMultipleCitation = false;

			if (citation == null) return null;
			if (citation.Reference == null) return null;
			if (componentPart == null) return null;
			if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			
			PersonFieldElement personFieldElement = componentPart.Elements.OfType<PersonFieldElement>().FirstOrDefault();
			if (personFieldElement == null) return null;
			

			//determine current persons to compare
			List<Person> thesePersons = GetPersonsDisplayed(personFieldElement, citation.Reference);
			bool usePlural = thesePersons.Count() > 1;

			PlaceholderCitation thisPlaceholderCitation = citation as PlaceholderCitation;

			#region deactivateFilterWithOption1Switch

			//SPECIAL: if this citation has the /opt1 switch set, this filter should be deactivated
			if (deactivateFilterWithOption1Switch && thisPlaceholderCitation != null && thisPlaceholderCitation.FormatOption1)
			{
				return null;
			}

			#endregion deactivateFilterWithOption1Switch

			//handle missing persons
			#region MissingPersonsOutput

			var text = string.Empty;
			var output = new TextUnitCollection();
			//SwissAcademic.Drawing.FontStyle fontStyle = outputInItalics ? SwissAcademic.Drawing.FontStyle.Italic : SwissAcademic.Drawing.FontStyle.Neutral;
			SwissAcademic.Drawing.FontStyle fontStyle;

			var personsMissing = (thesePersons == null || thesePersons.Count == 0);
			var outputMissingPersonsInfo = personsMissing && !string.IsNullOrEmpty(missingPersonsInfo);

			if (personsMissing && outputMissingPersonsInfo)
			{
				text = missingPersonsInfo;

				fontStyle = SwissAcademic.Drawing.FontStyle.Neutral;
				if (outputMissingPersonsInfoInItalics) fontStyle |= SwissAcademic.Drawing.FontStyle.Italic;
				if (outputMissingPersonsInfoInSmallCaps) fontStyle |= SwissAcademic.Drawing.FontStyle.SmallCaps;
				if (outputMissingPersonsInfoInBold) fontStyle |= SwissAcademic.Drawing.FontStyle.Bold;
				if (outputMissingPersonsInfoUnderlined) fontStyle |= SwissAcademic.Drawing.FontStyle.Underline;

				output.Add(new LiteralTextUnit(text, fontStyle));

				handled = true;
				return output;
			}
			else if (personsMissing)
			{
				return null;
			}

			#endregion MissingPersonsOutput

			var previousVisibleCitation = GetPreviousVisibleCitation(citation);
			if (previousVisibleCitation == null) return null;
			if (previousVisibleCitation.Reference == null) return null;

			var secondPreviousVisibleCitation = GetPreviousVisibleCitation(previousVisibleCitation);

			#region MultipleCitation

			if (thisPlaceholderCitation != null)
			{
				var printingEntries = thisPlaceholderCitation.Entry.Placeholder.GetPrintingEntries();
				if (printingEntries == null) return null;
				if (printingEntries.Count() > 1)
				{
					thisCitationIsPartOfMultipleCitation = true;
				}

				if (thisCitationIsPartOfMultipleCitation)
				{
					if (deactivateFilterInsideMultipleCitations)
					{
						//We switch off "ders./dies." completely ... or ...
						return null;
					}
					else
					{
						//... at least for the very first printing entry in a multiple citation
						var index = printingEntries.IndexOf(thisPlaceholderCitation.Entry);
						if (index != null && index == 0 && deactivateFilterForFirstInsideMultipleCitations) return null;
					}
				}
			}

			#endregion MultipleCitation

			#region deactivateFilterInIbidemIdemSequence

			//avoiding a sequence such as e.g.: [2] Ebd. -> [3] Ders.
			if (deactivateFilterInIbidemIdemSequence)
			{
				if (secondPreviousVisibleCitation != null && secondPreviousVisibleCitation.Reference != null)
				{
					if (previousVisibleCitation.Reference == secondPreviousVisibleCitation.Reference) return null;
				}
			}

			#endregion deactivateFilterInIbidemIdemSequence

			#region FootnoteCitation

			var thisFootnoteCitation = citation as FootnoteCitation;
			if (thisFootnoteCitation != null && !thisCitationIsPartOfMultipleCitation)
			{
				var previousVisibleFootnoteCitation = previousVisibleCitation as FootnoteCitation;
				if (previousVisibleFootnoteCitation == null) return null;

				var printingEntries = previousVisibleFootnoteCitation.Entry.Placeholder.GetPrintingEntries();
				if (printingEntries != null && printingEntries.Count() > 1 && deactivateFilterForFirstAfterMultipleCitations)
				{
					//previousVisibleFootnoteCitation IS part of a multiple citation
					return null;
				}

				int thisFootnoteIndex = thisFootnoteCitation.FootnoteIndex;
				int previousFootnoteIndex = previousVisibleFootnoteCitation.FootnoteIndex;

				var secondPreviousVisibleFootnoteCitation = secondPreviousVisibleCitation as FootnoteCitation;
				int secondPreviousFootnoteIndex = secondPreviousVisibleFootnoteCitation == null ? 0 : secondPreviousVisibleFootnoteCitation.FootnoteIndex;

				#region deactivateFilterAcrossFootnotes

				//enforce distance rules as given by user settings above
				if
				(
					(
						deactivateFilterAcrossFootnotes &&
						thisFootnoteIndex != previousFootnoteIndex
					) ||
					(
						!deactivateFilterAcrossFootnotes &&
						deactivateFilterAcrossFootnotesIfSeparatedByMoreThanOneIndexNo &&
						thisFootnoteIndex - previousFootnoteIndex > 1
					) ||
					(
						!deactivateFilterAcrossFootnotes &&
						deactivateFilterAcrossFootnotesIfPreviousCitationNotSolitair &&
						thisFootnoteIndex - previousFootnoteIndex == 1 &&
						secondPreviousFootnoteIndex == previousFootnoteIndex
					)
				)
				{
					return null;
				}

				#endregion deactivateFilterAcrossFootnotes
			}

			#endregion FootnoteCitation

			#region InTextCitation

			var thisInTextCitation = citation as InTextCitation;
			//if this citations predecessor is part of a multiple citation, but THIS is NOT, switch off filter
			if (thisInTextCitation != null && !thisCitationIsPartOfMultipleCitation)
			{
				var previousVisibleInTextCitation = previousVisibleCitation as InTextCitation;
				if (previousVisibleInTextCitation == null) return null;

				var printingEntries = previousVisibleInTextCitation.Entry.Placeholder.GetPrintingEntries();
				if (printingEntries != null && printingEntries.Count() > 1 && deactivateFilterForFirstAfterMultipleCitations)
				{
					//previousVisibleInTextCitation IS part of a multiple citation
					return null;
				}
			}

			#endregion InTextCitation

			//determine previous persons
			List<Person> previousPersons = GetPersonsDisplayed(previousVisibleCitation);
			if (previousPersons == null || previousPersons.Count == 0) return null;

			var equality = CheckPersonEquality(thesePersons, previousPersons);
			if (equality == PersonEquality.None) return null;

			#region Equality detected - generate output

			//we DO have some equality, so let's check what we need to output instead of the person's name(s)

			switch (equality)
			{
				case PersonEquality.M:
					text = "ders.";
					break;
					
				case PersonEquality.N:
					text = "dass.";
					break;
					
				default: //all others
					text = "dies.";
					break;

			}



			fontStyle = SwissAcademic.Drawing.FontStyle.Neutral;
			if (outputInItalics) fontStyle |= SwissAcademic.Drawing.FontStyle.Italic;
			if (outputInSmallCaps) fontStyle |= SwissAcademic.Drawing.FontStyle.SmallCaps;
			if (outputInBold) fontStyle |= SwissAcademic.Drawing.FontStyle.Bold;
			if (outputUnderlined) fontStyle |= SwissAcademic.Drawing.FontStyle.Underline;

			#region GroupPrefix

			if (showGroupPrefixIfPresent && !string.IsNullOrEmpty(personFieldElement.GroupPrefixPlural.Text) && usePlural)
			{
				output.Add(new LiteralTextUnit(personFieldElement.GroupPrefixPlural.Text, personFieldElement.GroupPrefixPlural.FontStyle));
			}
			else if (showGroupPrefixIfPresent && !string.IsNullOrEmpty(personFieldElement.GroupPrefixSingular.Text) && !usePlural)
			{
				output.Add(new LiteralTextUnit(personFieldElement.GroupPrefixSingular.Text, personFieldElement.GroupPrefixSingular.FontStyle));
			}

			#endregion GroupPrefix
			
			output.Add(new LiteralTextUnit(text, fontStyle));

			#region GroupSuffix

			if (showGroupSuffixIfPresent && !string.IsNullOrEmpty(personFieldElement.GroupSuffixPlural.Text) && usePlural)
			{
				output.Add(new LiteralTextUnit(personFieldElement.GroupSuffixPlural.Text, personFieldElement.GroupSuffixPlural.FontStyle));
			}
			else if (showGroupSuffixIfPresent && !string.IsNullOrEmpty(personFieldElement.GroupSuffixSingular.Text) && !usePlural)
			{
				output.Add(new LiteralTextUnit(personFieldElement.GroupSuffixSingular.Text, personFieldElement.GroupSuffixSingular.FontStyle));
			}

			#endregion GroupSuffix
			
			handled = true;
			return output;

			#endregion Equality detected - generate output
		}

		#region GetPreviousVisibleCitation

		private static Citation GetPreviousVisibleCitation(Citation citation)
		{
			if (citation == null) return null;

			#region Bibliography

			if (citation.CitationType == CitationType.Bibliography)
			{
				BibliographyCitation previousBibliographyCitation = citation as BibliographyCitation;
				if (previousBibliographyCitation == null) return null;

				//consider nobib
				do
				{
					previousBibliographyCitation = previousBibliographyCitation.PreviousBibliographyCitation;
					if (previousBibliographyCitation == null) return null;

				} while (previousBibliographyCitation.NoBib == true);

				//still here? found one!
				return previousBibliographyCitation;
			}

			#endregion Bibliography

			#region InText

			if (citation.CitationType == CitationType.InText)
			{
				InTextCitation previousInTextCitation = citation as InTextCitation;
				if (previousInTextCitation == null) return null;

				//consider bibonly
				do
				{
					previousInTextCitation = previousInTextCitation.PreviousInTextCitation;
					if (previousInTextCitation == null) return null;

				} while (previousInTextCitation.BibOnly == true);

				//still here? found one!
				return previousInTextCitation;
			}

			#endregion InText

			#region Footnote

			if (citation.CitationType == CitationType.Footnote)
			{
				FootnoteCitation previousFootnoteCitation = citation as FootnoteCitation;
				if (previousFootnoteCitation == null) return null;

				//consider bibonly
				do
				{
					previousFootnoteCitation = previousFootnoteCitation.PreviousFootnoteCitation;
					if (previousFootnoteCitation == null) return null;

				} while (previousFootnoteCitation.BibOnly == true);

				//still here? found one!
				return previousFootnoteCitation;
			}

			#endregion Footnote

			//still here? no visible previous citation found!
			return null;
		}

		#endregion GetPreviousCitation

		#region GetPersonsDisplayed

		private static List<Person> GetPersonsDisplayed(PersonFieldElement element, Reference reference)
		{
			List<Person> persons = null;
			if (element == null) return null;
			if (reference == null) return null;

			switch (element.PropertyId)
			{
				#region Authors

				case ReferencePropertyId.Authors:
					{
						if (reference.Authors != null) persons = new List<Person>(reference.Authors);
					}
					break;

				#endregion Authors

				#region Editors

				case ReferencePropertyId.Editors:
					{
						if (reference.Editors != null) persons = new List<Person>(reference.Editors);
					}
					break;

				#endregion Editors

				#region AuthorsEditorsOrganizations

				case ReferencePropertyId.AuthorsOrEditorsOrOrganizations:
					{
						if (reference.AuthorsOrEditorsOrOrganizations != null) persons = new List<Person>(reference.AuthorsOrEditorsOrOrganizations);
					}
					break;

				#endregion AuthorsEditorsOrganizations

				#region Collaborators

				case ReferencePropertyId.Collaborators:
					{
						if (reference.Collaborators != null) persons = new List<Person>(reference.Collaborators);
					}
					break;

				#endregion Collaborators

				#region Organizations

				case ReferencePropertyId.Organizations:
					{
						if (reference.Organizations != null) persons = new List<Person>(reference.Organizations);
					}
					break;

				#endregion Organizations

				#region OthersInvolved

				case ReferencePropertyId.OthersInvolved:
					{
						if (reference.OthersInvolved != null) persons = new List<Person>(reference.OthersInvolved);
					}
					break;

				#endregion OthersInvolved
			}

			return persons;
		}

		private static List<Person> GetPersonsDisplayed(ComponentPart componentPart, Reference reference)
		{
			List<Person> persons = null;
			if (reference == null) return null;
			if (componentPart == null) return null;

			//check for 1st PersonFieldElement in ComponentPart
			PersonFieldElement firstPersonFieldElement = componentPart.Elements.FirstOrDefault(item => item is PersonFieldElement) as PersonFieldElement;
			if (firstPersonFieldElement == null) return null;

			persons = GetPersonsDisplayed(firstPersonFieldElement, reference);
			return persons;
		}


		private static List<Person> GetPersonsDisplayed(Template template, Reference reference)
		{
			if (reference == null) return null;
			if (template == null) return null;
			if (template.ComponentParts == null || template.ComponentParts.Count == 0) return null;

			List<Person> persons = null;

			//check for 1st PersonFieldElement in citation's template
			IEnumerable<IElement> elements = template.ComponentParts.SelectMany(part => part.Elements);
			PersonFieldElement firstPersonFieldElement = elements.FirstOrDefault(item => item is PersonFieldElement) as PersonFieldElement;
			if (firstPersonFieldElement == null) return null;

			return GetPersonsDisplayed(firstPersonFieldElement, reference);
		}

		private static List<Person> GetPersonsDisplayed(Citation citation)
		{
			if (citation == null) return null;
			if (citation.Reference == null) return null;

			Template template = citation.GetTemplate();
			if (template == null) return null;

			List<Person> persons = null;

			persons = GetPersonsDisplayed(template, citation.Reference);
			if (persons != null) return persons;

			template = template.TemplateUseCase.FallbackTemplate;
			if (template == null) return null;
			persons = GetPersonsDisplayed(template, citation.Reference);
			if (persons != null) return persons;

			return null;
		}

		#endregion GetPersonsDisplayed

		#region CheckPersonEquality

		private static PersonEquality CheckPersonEquality(List<Person> personsA, List<Person> personsB)
		{
			if (personsA == null || personsA.Count == 0) return PersonEquality.None;
			if (personsB == null || personsB.Count == 0) return PersonEquality.None;
			if (personsA.Count != personsB.Count) return PersonEquality.None;

			//we DO have two lists of persons of same length
			//FIRST sort by id for comparison
			var personIdComparer = new PersonIdComparer();
			personsA.Sort(personIdComparer);
			personsB.Sort(personIdComparer);


			var allCounter = personsA.Count;
			var maleCounter = 0;
			var femaleCounter = 0;
			var neutralCounter = 0;

			//loop, compare GUID/id and determine/count sex 
			for (int i = 0; i < personsA.Count; i++)
			{
				var idA = personsA[i].GetValue(PersonPropertyId.Id).ToString();
				var idB = personsB[i].GetValue(PersonPropertyId.Id).ToString();

				if (!idA.Equals(idB, StringComparison.Ordinal)) return PersonEquality.None;

				//identical!
				//determine sex (just need to look at one of them, because they are identical)
				if (personsA[i].Sex == Sex.Male) maleCounter++;
				if (personsA[i].Sex == Sex.Female) femaleCounter++;
				if (personsA[i].Sex == Sex.Neutral || personsA[i].Sex == Sex.Unknown) neutralCounter++;
			}

			//still here, so ALL persons are equal, now return equality based on sex
			if (allCounter == 1 && maleCounter == 1) return PersonEquality.M;
			else if (allCounter == 1 && femaleCounter == 1) return PersonEquality.F;
			else if (allCounter == 1 && neutralCounter == 1) return PersonEquality.N;

			else if (allCounter > 1 && maleCounter == allCounter) return PersonEquality.MM;
			else if (allCounter > 1 && femaleCounter == allCounter) return PersonEquality.FF;
			else if (allCounter > 1 && neutralCounter == allCounter) return PersonEquality.NN;

			else if (allCounter > 1 && maleCounter + femaleCounter == allCounter) return PersonEquality.FM;
			else if (allCounter > 1 && femaleCounter + neutralCounter == allCounter) return PersonEquality.FN;
			else if (allCounter > 1 && maleCounter + neutralCounter == allCounter) return PersonEquality.MN;

			else if (allCounter >= 3
				&& maleCounter >= 1 && femaleCounter >= 1 && neutralCounter >= 1
				&& maleCounter + femaleCounter + neutralCounter == allCounter) return PersonEquality.FMN;
			else return PersonEquality.None;

		}

		#endregion CheckPersonEquality

		#region Enum PersonEquality

		public enum PersonEquality
		{
			/// <summary>
			/// None: Different persons and/or different numbers of persons.
			/// </summary>
			/// <returns></returns>
			None,

			/// <summary>
			/// Identical person, a single female (Latin: eadem)
			/// </summary>
			F,
			/// <summary>
			/// Identical person, a single male (Latin: idem)
			/// </summary>
			M,
			/// <summary>
			/// Identical persons, a single (neutral) organization (Latin: idem)
			/// </summary>
			N,

			/// <summary>
			/// Identical persons, only females, 2 or more (Latin: eaedem)
			/// </summary>
			FF,
			/// <summary>
			/// Identical persons, only males, 2 or more (Latin: eidem)
			/// </summary>
			MM,
			/// <summary>
			/// Identical persons, only (neutral) organizations, 2 or more (Latin: eadem)
			/// </summary>
			NN,

			/// <summary>
			/// Identical persons, mixed group of females and males only
			/// </summary>
			FM,
			/// <summary>
			/// Identical persons, mixed group of females and neutrals only
			/// </summary>
			FN,
			/// <summary>
			/// Identical persons, mixed group of males and neutrals only
			/// </summary>
			MN,

			/// <summary>
			/// Identical persons, mixed group of females, males and neutrals
			/// </summary>
			FMN
		}

		#endregion PersonEquality

		#region PersonIdComparer

		//The following is a sort comparer that will bring all person collections into a well defined order
		//namely in the order of their internal GUID values.
		public class PersonIdComparer : IComparer<Person>
		{
			public int Compare(Person person1, Person person2)
			{
				int returnValue = 1;
				if (person1 != null && person2 != null)
				{
					returnValue = person1.GetValue(PersonPropertyId.Id).ToString().CompareTo(person2.GetValue(PersonPropertyId.Id).ToString());
				}
				return returnValue;
			}
		}

		#endregion PersonIdComparer

	}
}
