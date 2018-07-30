//C6#CPS001
//C5#43111
//Description: Author of contribution is also editor of compilation - ders, dies
//Version: 3.2  
//Version: 3.2 See note under 3.0 + jumps over 'empty' person fields
//Version: 3.1 Corrects error that "ders./dies./ibid." was always displayed
//Version: 3.0 Compares Persons by checking surrounding template (complete revision)
//Version: 2.0 Now works with GroupPrefix and GroupSuffix as well as additional LiteralElements or FieldElements inside the component part
//Version: 1.5 In case of "Contribution in Collected Works", we have to check for "Author (of contribution ) = Author (of parent reference)" 
//            and not for "Author (of contribution) = Editor (of parent reference). See code line 40.
//Version: 1.4 New variable "outputInSmallCaps", "outputInBold", "outputUnderlined"
//Version: 1.3 Single N oder unknown author yields "ders." instead of "dies."
//added outputInItalics variable on top

using System;
using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;

namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
		:
		IComponentPartFilter
	{

		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			//change the following to "true", if you want "ders."/"dies." written in italics or other font styles
			var outputInItalics = true;
			var outputInSmallCaps = false;
			var outputInBold = false;
			var outputUnderlined = false;

			//NOTE: If you want a prefix such as "In: " and a suffix " (Hrsg)", you can define them as group prefix and suffix on the field element inside the component part editor

			handled = false;

            if (componentPart == null) return null;
            if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;

            if (citation == null) return null;

            Reference reference = citation.Reference;
            if (reference == null) return null;

            Reference parentReference = citation.Reference.ParentReference;
			

            #region ThisPersonFieldElement

            PersonFieldElement thisPersonFieldElement = GetPersonFieldElement(componentPart);
            if (thisPersonFieldElement == null) return null;

            #endregion

            #region ThesePersons

            IList<Person> thesePersons = GetPersonsDisplayed(thisPersonFieldElement, reference);

            #endregion
			

            #region PreviousPersonFieldElement

			PersonFieldElement previousPersonFieldElement = GetPreviousPersonFieldElement(thisPersonFieldElement, template, reference);
			if (previousPersonFieldElement == null) return null;

            #endregion
			
            #region PreviousPersons

            IList<Person> previousPersons = GetPersonsDisplayed(previousPersonFieldElement, reference);
			if (previousPersons == null || !previousPersons.Any()) return null;

            #endregion



            bool usePlural = thesePersons.Count() > 1;
			
            PersonEquality equality = CheckPersonEquality(thesePersons, previousPersons);			
            if (equality == PersonEquality.None) return null;

            //we DO have some equality, so let's check what we need to output instead of the person's name(s)
            var textIdem = string.Empty;
			switch (equality)
			{
				case PersonEquality.M:
				case PersonEquality.N:
					textIdem = "ders.";
					break;

				default: //all others
					textIdem = "dies.";
					break;
			}


			TextUnitCollection output = new TextUnitCollection();

			#region GroupPrefix

			if (usePlural && !string.IsNullOrEmpty(thisPersonFieldElement.GroupPrefixPlural.Text))
			{
				output.Add(new LiteralTextUnit(thisPersonFieldElement.GroupPrefixPlural.Text, thisPersonFieldElement.GroupPrefixPlural.FontStyle));
			}
			else if (!usePlural & !string.IsNullOrEmpty(thisPersonFieldElement.GroupPrefixSingular.Text))
			{
				output.Add(new LiteralTextUnit(thisPersonFieldElement.GroupPrefixSingular.Text, thisPersonFieldElement.GroupPrefixSingular.FontStyle));
			}

			#endregion GroupPrefix

			SwissAcademic.Drawing.FontStyle fontStyle;
			fontStyle = SwissAcademic.Drawing.FontStyle.Neutral;
			if (outputInItalics) fontStyle |= SwissAcademic.Drawing.FontStyle.Italic;
			if (outputInSmallCaps) fontStyle |= SwissAcademic.Drawing.FontStyle.SmallCaps;
			if (outputInBold) fontStyle |= SwissAcademic.Drawing.FontStyle.Bold;
			if (outputUnderlined) fontStyle |= SwissAcademic.Drawing.FontStyle.Underline;

			var fontStyleNeutral = SwissAcademic.Drawing.FontStyle.Neutral;

			output.Add(new LiteralTextUnit(textIdem, fontStyle));

			#region GroupSuffix

			if (usePlural && !string.IsNullOrEmpty(thisPersonFieldElement.GroupSuffixPlural.Text))
			{
				output.Add(new LiteralTextUnit(thisPersonFieldElement.GroupSuffixPlural.Text, thisPersonFieldElement.GroupSuffixPlural.FontStyle));
			}
			else if (!usePlural && !string.IsNullOrEmpty(thisPersonFieldElement.GroupSuffixSingular.Text))
			{
				output.Add(new LiteralTextUnit(thisPersonFieldElement.GroupSuffixSingular.Text, thisPersonFieldElement.GroupSuffixSingular.FontStyle));
			}

			#endregion GroupSuffix


			//inject this as LiteralElements into the componentPart, replacing the editors person field element
			componentPart.Elements.ReplaceItem(thisPersonFieldElement, TextUnitsToLiteralElements(output, componentPart)); //for some reason this does not work

            //all literal elements should always be output:
            foreach (LiteralElement literalElement in componentPart.Elements.OfType<LiteralElement>())
            {
                literalElement.ApplyCondition = ElementApplyCondition.Always;
            }

            handled = false;
			return null;

		}

		#region CheckPersonEquality

		private static PersonEquality CheckPersonEquality(IList<Person> personsACollection, IList<Person> personsBCollection)
		{
			if (personsACollection == null || personsACollection.Count == 0) return PersonEquality.None;
			if (personsBCollection == null || personsBCollection.Count == 0) return PersonEquality.None;
			if (personsACollection.Count != personsBCollection.Count) return PersonEquality.None;

			//we DO have two lists of persons of same length
			//FIRST sort by id for comparison
			var personIdComparer = new PersonIdComparer();
			
			List<Person> personsA = personsACollection.ToList();
			List<Person> personsB = personsBCollection.ToList();
			
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
		
		#region GetPersonFieldElement
		
		private static PersonFieldElement GetPersonFieldElement(ComponentPart componentPart)
		{
			if (componentPart == null) return null;
			if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			
			return componentPart.Elements.OfType<PersonFieldElement>().FirstOrDefault() as PersonFieldElement;
		}
		
		#endregion
		
		#region GetPreviousPersonFieldElement
		
		private static PersonFieldElement GetPreviousPersonFieldElement(PersonFieldElement thisPersonFieldElement, Template template, Reference reference)
		{
			if (thisPersonFieldElement == null) return null;
            if (template == null || template.ComponentParts == null || !template.ComponentParts.Any()) return null;

            IEnumerable<PersonFieldElement> allPersonFieldElements = template.ComponentParts.SelectMany(part => part.Elements).OfType<PersonFieldElement>().ToList();
            if (allPersonFieldElements == null || !allPersonFieldElements.Any()) return null;
		
			int thisIndex = allPersonFieldElements.FindIndex<PersonFieldElement>(item => item.Id == thisPersonFieldElement.Id);
			if (thisIndex == -1 || thisIndex == 0) return null;
			
			for (int i = thisIndex - 1; i >= 0; i--)
			{
				PersonFieldElement previousPersonFieldElement = allPersonFieldElements.ElementAtOrDefault<PersonFieldElement>(i) as PersonFieldElement;
				if (previousPersonFieldElement == null) continue;
				
				List<Person> previousPersons = GetPersonsDisplayed(previousPersonFieldElement, reference);
				if (previousPersons == null || !previousPersons.Any()) continue;
				
				return previousPersonFieldElement;
			}
			
			return null;
		}
		
		#endregion
		
		#region GetPersonsDisplayed
		
		private static List<Person> GetPersonsDisplayed(PersonFieldElement element, Reference reference)
		{
			if (element == null) return null;
			if (element.ComponentPart == null) return null;
			List<Person> persons = null;
			
			ComponentPartScope scope = element.ComponentPart.Scope;
			Reference referenceInScope = null;
			if (scope == ComponentPartScope.Reference) referenceInScope = reference;
			else referenceInScope = reference.ParentReference;
			if (referenceInScope == null) return null;

			switch (element.PropertyId)
			{
				#region Authors

				case ReferencePropertyId.Authors:
					{
						if (referenceInScope.Authors != null) persons = new List<Person>(referenceInScope.Authors);
					}
					break;

				#endregion Authors

				#region Editors

				case ReferencePropertyId.Editors:
					{
						if (referenceInScope.Editors != null) persons = new List<Person>(referenceInScope.Editors);
					}
					break;

				#endregion Editors

				#region AuthorsEditorsOrganizations

				case ReferencePropertyId.AuthorsOrEditorsOrOrganizations:
					{
						if (referenceInScope.AuthorsOrEditorsOrOrganizations != null) persons = new List<Person>(referenceInScope.AuthorsOrEditorsOrOrganizations);
					}
					break;

				#endregion AuthorsEditorsOrganizations

				#region Collaborators

				case ReferencePropertyId.Collaborators:
					{
						if (referenceInScope.Collaborators != null) persons = new List<Person>(referenceInScope.Collaborators);
					}
					break;

				#endregion Collaborators

				#region Organizations

				case ReferencePropertyId.Organizations:
					{
						if (referenceInScope.Organizations != null) persons = new List<Person>(referenceInScope.Organizations);
					}
					break;

				#endregion Organizations

				#region OthersInvolved

				case ReferencePropertyId.OthersInvolved:
					{
						if (referenceInScope.OthersInvolved != null) persons = new List<Person>(referenceInScope.OthersInvolved);
					}
					break;

				#endregion OthersInvolved
			}


			return persons;
		}
		
		#endregion

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

		#region TextUnitsToLiteralElements

		IEnumerable<IElement> TextUnitsToLiteralElements(TextUnitCollection textUnits, ComponentPart componentPart)
		{
			if (componentPart == null) yield break;
			if (textUnits == null || textUnits.Count == 0) yield break;

			foreach (ITextUnit textUnit in textUnits)
			{
				LiteralElement element = new LiteralElement(componentPart, textUnit.Text, ElementApplyCondition.Always);
				element.FontStyle = textUnit.FontStyle;
				yield return element;
			}
		}

		#endregion TextUnitsToLiteralElements

	}
}