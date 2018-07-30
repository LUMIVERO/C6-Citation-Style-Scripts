//C6#CPS018
//Description: Add initials for first author only
//Version: 1.0
//Used in e.g.: APA 6

using System;
using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;

namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
		:
		IComponentPartFilter
	{
		//Version 1.0a Add initials if there are other authors with the same last name (FIRST PERSON ONLY)
		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			PersonNameOrder nameOrderForAmbiguityResolution = PersonNameOrder.LastNameFirstName;
			NameFormat firstNameFormatForAmbiguityResolution = NameFormat.Abbreviated;
			
			handled = false;

			if (componentPart == null) return null;
			if (citation == null || citation.CitationManager == null) return null;
			
			Reference reference = citation.Reference;
			if (reference == null) return null;

			PersonFieldElement personFieldElement = componentPart.GetFieldElements().OfType<PersonFieldElement>().FirstOrDefault();
			if (personFieldElement == null) return null;
			
			IEnumerable<Person> persons = personFieldElement.GetPersons(reference);
			if (persons == null || !persons.Any()) return null;
			
			Guid firstPersonId = persons.First<Person>().Id;
		
			#region BeforeFormatPerson
			
			BeforeFormatPersonEventArgs b;
			personFieldElement.PersonFormatter.BeforeFormatPerson += 
			(sender, e) =>
			{
				b = (BeforeFormatPersonEventArgs)e;
				if (b.Person == null) return;
				if (!b.Person.Id.Equals(firstPersonId)) return;
				
				if (citation.CitationManager.IsCitedPersonLastNameAmbiguous(b.Person.LastName))
				{
					b.NameOrder = nameOrderForAmbiguityResolution;
					b.FirstNameFormat = firstNameFormatForAmbiguityResolution;
				}
			};
			
			#endregion
			
			return null;
		}
	}
}