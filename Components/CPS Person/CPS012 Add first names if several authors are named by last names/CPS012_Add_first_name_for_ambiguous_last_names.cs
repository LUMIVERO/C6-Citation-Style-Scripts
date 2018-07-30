// CPS012
//Version 3.4
//Version 3.4 Get rid of person cloneing plus small improvements
//Version 3.3 Copy font style form first group last name to organization name to ensure also "expanded" last names are formatted correctly
//Version 3.2 In case of NameIdentity.LastName wie use the new output PersonFieldElementFirstNameInitialMiddleNameInitial
//Version 3.1 Script is deactivated when used on a placeholder citation with /yearonly option ONLY if there is a year field present in the template
//Version 3.0 Script can deal with different forms of name identity: LastName, LastNameFirstNameInitial, LastNameFirstNameFull etc.
//Version 2.2 Script allows for combination with idem/eadem output if the same person or group of persons is repeated
//Version 2.1 Script is deactivated when used on a placeholder citation with /yearonly option
//Show first name if last names are identical for different persons

using System;
using System.Collections.Generic;
using System.Linq;

namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
		:
		IComponentPartFilter
	{

        public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			bool useNonBreakingSpace = false;
			bool useNonBreakingHyphen = false;

			handled = false;

			if (template == null) return null;
			if (citation == null) return null;
			if (citation.CitationManager == null) return null;

			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation != null && placeholderCitation.YearOnly && template.StructuralDateTimeFieldElement != null) return null;

			var reference = citation.Reference;
			if (reference == null) return null;

			if (componentPart == null) return null;
			if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;

			PersonFieldElement personFieldElement = componentPart.Elements.OfType<PersonFieldElement>().FirstOrDefault();
			if (personFieldElement == null) return null;

			bool duplicateLastNameFound = false;
			List<Person> personsForOutput = GetClonedPersonsForOutput(personFieldElement, citation, out duplicateLastNameFound);
			if (!duplicateLastNameFound) return null;
			if (personsForOutput == null || !personsForOutput.Any()) return null;

			bool usePlural = personsForOutput.Count > 1;
			TextUnitCollection personTextUnits = null;

            //cloned persons are recognized as organizations because we add the first name to the last name
            personFieldElement.OrganizationNameFontStyle = personFieldElement.FirstGroupLastNameFontStyle;

            personTextUnits = personFieldElement.PersonFormatter.Format(personsForOutput, citation);

			TextUnitCollection output = new TextUnitCollection();
            if (personTextUnits != null && personTextUnits.Any())
			{
				personTextUnits.ForEach(u =>
				{
					if (u.Text.Length > 3)
					{
						if (useNonBreakingSpace) u.Text = u.Text.Replace(StringUtility.Space, StringUtility.NonBreakingSpace);
						if (useNonBreakingHyphen) u.Text = u.Text.Replace(StringUtility.Hyphen, StringUtility.NonBreakingHyphen);
					}
				});

				output.AddRange(personTextUnits);
				componentPart.Elements.ReplaceItem(personFieldElement, TextUnitCollectionUtility.TextUnitsToLiteralElements(output, componentPart));
			}

			return null;
		}

		#region GetClonedPersonsForOutput

		List<Person> GetClonedPersonsForOutput(PersonFieldElement personFieldElement, Citation citation, out bool duplicateLastNameFound)
		{
			duplicateLastNameFound = false;

			if (personFieldElement == null) return null;
			if (citation == null) return null;
			if (citation.Reference == null) return null;

			List<Person> originalPersons = personFieldElement.GetPersons(citation).ToList(); //takes scope of ComponentPart into account
			if (originalPersons == null || !originalPersons.Any()) return null;

			//use the following for configuration of enhanced name output incl. first names, middle names etc.
			#region PersonFieldElementFirstNameInitial

			PersonFieldElement personFieldElementFirstNameInitial = new PersonFieldElement(personFieldElement.ComponentPart, personFieldElement.PropertyId);
			personFieldElementFirstNameInitial.FirstGroupFirstNameFormat = NameFormat.Abbreviated;
			personFieldElementFirstNameInitial.FirstGroupPersonNameOrder = PersonNameOrder.FirstNameLastName;
			personFieldElementFirstNameInitial.FirstGroupLastNameFontStyle = personFieldElement.FirstGroupLastNameFontStyle;
			personFieldElementFirstNameInitial.FirstGroupMiddleNameUsage = MiddleNameUsage.None;

			#endregion

			#region PersonFieldElementFirstNameFull

			PersonFieldElement personFieldElementFirstNameFull = new PersonFieldElement(personFieldElement.ComponentPart, personFieldElement.PropertyId);
			personFieldElementFirstNameFull.FirstGroupFirstNameFormat = NameFormat.Full;
			personFieldElementFirstNameFull.FirstGroupPersonNameOrder = PersonNameOrder.FirstNameLastName;
			personFieldElementFirstNameFull.FirstGroupLastNameFontStyle = personFieldElement.FirstGroupLastNameFontStyle;
			personFieldElementFirstNameInitial.FirstGroupMiddleNameUsage = MiddleNameUsage.None;

			#endregion
			
			#region PersonFieldElementFirstNameInitialMiddleNameInitial
			
			PersonFieldElement personFieldElementFirstNameInitialMiddleNameInitial = new PersonFieldElement(personFieldElement.ComponentPart, personFieldElement.PropertyId);
			personFieldElementFirstNameInitialMiddleNameInitial.FirstGroupFirstNameFormat = NameFormat.Abbreviated;
			personFieldElementFirstNameInitialMiddleNameInitial.FirstGroupPersonNameOrder = PersonNameOrder.FirstNameLastName;
			personFieldElementFirstNameInitialMiddleNameInitial.FirstGroupMiddleNameFormat = NameFormat.Abbreviated;
			personFieldElementFirstNameInitialMiddleNameInitial.FirstGroupMiddleNameUsage = MiddleNameUsage.All;
			personFieldElementFirstNameInitialMiddleNameInitial.FirstGroupLastNameFontStyle = personFieldElement.FirstGroupLastNameFontStyle;
			
			#endregion

			#region PersonFieldElementFirstNameFullMiddleNameInitial

			PersonFieldElement personFieldElementFirstNameFullMiddleNameInitial = new PersonFieldElement(personFieldElement.ComponentPart, personFieldElement.PropertyId);
			personFieldElementFirstNameFullMiddleNameInitial.FirstGroupFirstNameFormat = NameFormat.Full;
			personFieldElementFirstNameFullMiddleNameInitial.FirstGroupPersonNameOrder = PersonNameOrder.FirstNameLastName;
			personFieldElementFirstNameFullMiddleNameInitial.FirstGroupMiddleNameFormat = NameFormat.Abbreviated;
			personFieldElementFirstNameFullMiddleNameInitial.FirstGroupMiddleNameUsage = MiddleNameUsage.All;
			personFieldElementFirstNameFullMiddleNameInitial.FirstGroupLastNameFontStyle = personFieldElement.FirstGroupLastNameFontStyle;

			#endregion

			#region PersonFieldElementFirstNameFullMiddleNameFull

			PersonFieldElement personFieldElementFirstNameFullMiddleNameFull = new PersonFieldElement(personFieldElement.ComponentPart, personFieldElement.PropertyId);
			personFieldElementFirstNameFullMiddleNameFull.FirstGroupFirstNameFormat = NameFormat.Full;
			personFieldElementFirstNameFullMiddleNameFull.FirstGroupPersonNameOrder = PersonNameOrder.FirstNameLastName;
			personFieldElementFirstNameFullMiddleNameFull.FirstGroupMiddleNameFormat = NameFormat.Full;
			personFieldElementFirstNameFullMiddleNameFull.FirstGroupMiddleNameUsage = MiddleNameUsage.All;
			personFieldElementFirstNameFullMiddleNameFull.FirstGroupLastNameFontStyle = personFieldElement.FirstGroupLastNameFontStyle;

			#endregion



			List<Person> outputPersons = new List<Person>();

			foreach (Person person in originalPersons)
			{
				switch (CheckNameIdentity(person, citation))
				{
					#region NameIdentity.LastNameFirstNameFullMiddleNameInitial

					case NameIdentity.LastNameFirstNameFullMiddleNameInitial:
						{
							duplicateLastNameFound = true;

                            string newDisplayName = personFieldElementFirstNameFullMiddleNameFull.PersonFormatter.FormatSinglePerson(person, PersonFieldGroup.First).ToString();
                            Person disambiguatedPerson = new Person(person.Project, newDisplayName);

							outputPersons.Add(disambiguatedPerson);
						}
						break;

					#endregion

					#region NameIdentity.LastNameFirstNameFull

					case NameIdentity.LastNameFirstNameFull:
						{
							duplicateLastNameFound = true;

                            string newDisplayName = personFieldElementFirstNameFullMiddleNameInitial.PersonFormatter.FormatSinglePerson(person, PersonFieldGroup.First).ToString();
                            Person disambiguatedPerson = new Person(person.Project, newDisplayName);

							outputPersons.Add(disambiguatedPerson);
						}
						break;

					#endregion

					#region NameIdentity.LastNameFirstNameInitial

					case NameIdentity.LastNameFirstNameInitial:
						{
							duplicateLastNameFound = true;

							string newDisplayName = personFieldElementFirstNameFullMiddleNameInitial.PersonFormatter.FormatSinglePerson(person, PersonFieldGroup.First).ToString();
                            Person disambiguatedPerson = new Person(person.Project, newDisplayName);


                            outputPersons.Add(disambiguatedPerson);
						}
						break;

					#endregion

					#region NameIdentity.LastName

					case NameIdentity.LastName:
						{
							duplicateLastNameFound = true;

							
							string newDisplayName = personFieldElementFirstNameInitialMiddleNameInitial.PersonFormatter.FormatSinglePerson(person, PersonFieldGroup.First).ToString();
                            Person disambiguatedPerson = new Person(person.Project, newDisplayName);

                            outputPersons.Add(disambiguatedPerson);
						}
						break;

					#endregion

					#region Default

					default:
					case NameIdentity.None:
					case NameIdentity.LastNameFirstNameFullMiddleNameFull: //we return to just last name, because there is nothing we can do here
						{
							outputPersons.Add(person);
						}
						break;

					#endregion
				}
			}
			return outputPersons;
		}

		#endregion GetClonedPersonsForOutput

		#region CheckNameIdentity

		enum NameIdentity
		{
			None,
			LastName,
			LastNameFirstNameInitial,
			LastNameFirstNameFull,
			LastNameFirstNameFullMiddleNameInitial,
			LastNameFirstNameFullMiddleNameFull
		}

		NameIdentity CheckNameIdentity(Person currentPerson, Citation currentCitation)
		{
			if (currentCitation == null) return NameIdentity.None;
			if (currentCitation.CitationManager == null) return NameIdentity.None;

			var placeholderCitations = currentCitation.CitationManager.PlaceholderCitations;
			if (placeholderCitations == null || !placeholderCitations.Any()) return NameIdentity.None;

			List<Person> citedPersons = (
				from citation in placeholderCitations
				where citation.Template != null
				from personField in citation.Template.PersonFieldElements
				from person in personField.GetPersonsCited(citation)
				select person
			).Distinct().ToList();


			List<Person> personsWithIdenticalLastName = citedPersons.Where(p => string.Equals(p.LastName, currentPerson.LastName, StringComparison.OrdinalIgnoreCase) && p != currentPerson).ToList();
			List<Person> personsWithIdenticalLastNameFirstNameInitial = new List<Person>();
			List<Person> personsWithIdenticalLastNameFirstNameFull = new List<Person>();
			List<Person> personsWithIdenticalLastNameFirstNameFullMiddleNameInitial = new List<Person>();
			List<Person> personsWithIdenticalLastNameFirstNameFullMiddleNameFull = new List<Person>();


			if (!string.IsNullOrEmpty(currentPerson.FirstName))
			{
				personsWithIdenticalLastNameFirstNameInitial = (
					from person in personsWithIdenticalLastName
					where !string.IsNullOrEmpty(person.FirstName) && person.FirstName[0] == currentPerson.FirstName[0]
					select person
				).ToList();

				personsWithIdenticalLastNameFirstNameFull = (
					from person in personsWithIdenticalLastNameFirstNameInitial
					where string.Equals(person.FirstName, currentPerson.FirstName, StringComparison.OrdinalIgnoreCase)
					select person
				).ToList();
			}

			if (!string.IsNullOrEmpty(currentPerson.MiddleName))
			{
				personsWithIdenticalLastNameFirstNameFullMiddleNameInitial = (
					from person in personsWithIdenticalLastNameFirstNameFull
					where !string.IsNullOrEmpty(person.MiddleName) && person.MiddleName[0] == currentPerson.MiddleName[0]
					select person
				).ToList();

				personsWithIdenticalLastNameFirstNameFullMiddleNameFull = (
					from person in personsWithIdenticalLastNameFirstNameFullMiddleNameInitial
					where string.Equals(person.MiddleName, currentPerson.MiddleName, StringComparison.OrdinalIgnoreCase)
					select person
				).ToList();
			}

			if (personsWithIdenticalLastNameFirstNameFullMiddleNameFull.Any()) return NameIdentity.LastNameFirstNameFullMiddleNameFull;
			if (personsWithIdenticalLastNameFirstNameFullMiddleNameInitial.Any()) return NameIdentity.LastNameFirstNameFullMiddleNameInitial;

			if (personsWithIdenticalLastNameFirstNameFull.Any()) return NameIdentity.LastNameFirstNameFull;
			if (personsWithIdenticalLastNameFirstNameInitial.Any()) return NameIdentity.LastNameFirstNameInitial;

			if (personsWithIdenticalLastName.Any()) return NameIdentity.LastName;

			return NameIdentity.None; //?
		}

		#endregion OtherPersonWithSameLastNameAlsoCited
		
	}
}