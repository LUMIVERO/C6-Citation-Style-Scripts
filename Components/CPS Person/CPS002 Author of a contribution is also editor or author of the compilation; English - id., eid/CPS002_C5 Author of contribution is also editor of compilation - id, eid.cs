//C6#CPS02
//C5#43111
//Description: Author of contribution is also editor of compilation - id., eid.
//Version: 2.0
//Version: 2.0 Now works with GroupPrefix and GroupSuffix as well as additional LiteralElements or FieldElements inside the component part
//Version: 1.4 In case of "Contribution in Collected Works", we have to check for "Author (of contribution ) = Author (of parent reference)" 
//            and not for "Author (of contribution) = Editor (of parent reference). See code line 40.
//Version: 1.3 New variable "outputInSmallCaps", "outputInBold", "outputUnderlined"
//			  A group of several males is now "iid." instead of "eid."

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
			//change the following to "true", if you want "id.", "eid.", "ead.", "eaed." written in italics or other font styles
			bool outputInItalics = true;
			bool outputInSmallCaps = false;
			bool outputInBold = false;
			bool outputUnderlined = false;

			//NOTE: If you want a prefix such as "In: " and a suffix " (Hrsg)", you can define them as group prefix and suffix on the field element inside the component part editor

			handled = false;

			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;

			if (citation == null) return null;

			Reference reference = citation.Reference;
			if (reference == null) return null;

			Reference parentReference = reference.ParentReference;
			if (parentReference == null) return null;


			//get editor person field elements (or authors in case of CollectedWorks) and make it a separate List<>, since we are going to change that collection below
			List<PersonFieldElement> editorPersonFieldElements =
				parentReference.ReferenceType == ReferenceType.CollectedWorks ?
				componentPart.Elements.OfType<PersonFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.Authors).ToList() :
				componentPart.Elements.OfType<PersonFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.Editors).ToList();

			if (editorPersonFieldElements == null || editorPersonFieldElements.Count() == 0) return null;



			//we DO have a valid citation/reference that is a parent's child
			ReferencePersonCollection childAuthorsCollection = reference.Authors;
			if (childAuthorsCollection == null || childAuthorsCollection.Count == 0) return null;
			List<Person> childAuthors = new List<Person>(childAuthorsCollection);

			ReferencePersonCollection parentEditorsCollection = parentReference.ReferenceType == ReferenceType.CollectedWorks ? parentReference.Authors : parentReference.Editors;
			if (parentEditorsCollection == null || parentEditorsCollection.Count == 0) return null;
			List<Person> parentEditors = new List<Person>(parentEditorsCollection);

			bool usePlural = parentEditors.Count() > 1;

			PersonEquality equality = CheckPersonEquality(childAuthors, parentEditors);
			if (equality == PersonEquality.None) return null;


			//we DO have some equality, so let's check what we need to output instead of the person's name(s)
			var textIdem = string.Empty;
			switch (equality)
			{
				//see http://en.wiktionary.org/wiki/idem#Inflection
				case PersonEquality.M:
				case PersonEquality.N:
					textIdem = "id.";
					break;

				case PersonEquality.F:
					textIdem = "ead.";
					break;

				case PersonEquality.NN:
					textIdem = "ead.";
					break;

				case PersonEquality.MM:
					textIdem = "iid.";
					break;

				case PersonEquality.MN:
				case PersonEquality.FM:
				case PersonEquality.FMN:
					textIdem = "eid.";
					break;

				case PersonEquality.FF:
				case PersonEquality.FN:
					textIdem = "eaed.";
					break;
			}

			foreach (PersonFieldElement editors in editorPersonFieldElements)
			{
				TextUnitCollection output = new TextUnitCollection();

				#region GroupPrefix

				if (usePlural && !string.IsNullOrEmpty(editors.GroupPrefixPlural.Text))
				{
					output.Add(new LiteralTextUnit(editors.GroupPrefixPlural.Text, editors.GroupPrefixPlural.FontStyle));
				}
				else if (!usePlural & !string.IsNullOrEmpty(editors.GroupPrefixSingular.Text))
				{
					output.Add(new LiteralTextUnit(editors.GroupPrefixSingular.Text, editors.GroupPrefixSingular.FontStyle));
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

				if (usePlural && !string.IsNullOrEmpty(editors.GroupSuffixPlural.Text))
				{
					output.Add(new LiteralTextUnit(editors.GroupSuffixPlural.Text, editors.GroupSuffixPlural.FontStyle));
				}
				else if (!usePlural && !string.IsNullOrEmpty(editors.GroupSuffixSingular.Text))
				{
					output.Add(new LiteralTextUnit(editors.GroupSuffixSingular.Text, editors.GroupSuffixSingular.FontStyle));
				}

				#endregion GroupSuffix


				//inject this as LiteralElements into the componentPart, replacing the editors person field element
				componentPart.Elements.ReplaceItem(editors, TextUnitsToLiteralElements(output, componentPart)); //for some reason this does not work
			}

			handled = false;
			return null;

		}

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