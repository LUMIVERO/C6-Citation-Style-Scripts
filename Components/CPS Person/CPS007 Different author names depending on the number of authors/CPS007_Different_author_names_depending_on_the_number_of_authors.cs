// #C5_43118
//Version: 1.0

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
		//Unterschiedliche Ausgabe von 1, 2, 3 oder mehr Autoren
		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			handled = false;

			if (citation == null) return null;

			var reference = citation.Reference;
			if (reference == null) return null;

			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;

			var firstPersonFieldElement = componentPart.Elements.FirstOrDefault(item => item is PersonFieldElement) as PersonFieldElement;
			if (firstPersonFieldElement == null) return null;

			var displayedPersons = GetPersonsDisplayed(firstPersonFieldElement, reference);
			if (displayedPersons == null || displayedPersons.Count == 0) return null;

			//general settings
			firstPersonFieldElement.FirstGroupLength = -1; //all
			firstPersonFieldElement.FirstGroupFirstNameFormat = NameFormat.Full;
			firstPersonFieldElement.FirstGroupMiddleNameUsage = MiddleNameUsage.None;

			firstPersonFieldElement.AbbreviateIfMoreThanPersons = 3;
			firstPersonFieldElement.AbbreviateUpToPerson = 1;

			firstPersonFieldElement.FirstGroupPersonSeparator.Text = "/";


			//Name1/Name2/Name3
			if (displayedPersons.Count() == 3)
			{
				firstPersonFieldElement.FirstGroupPersonNameOrder = PersonNameOrder.LastNameOnly;

			}

			//Name1, Vorname 1
			//Name1, Vorname 1/Name2, Vorname 2
			//Name1, Vorname 1 et al. (more than 3 authors)
			else
			{
				firstPersonFieldElement.FirstGroupPersonNameOrder = PersonNameOrder.LastNameFirstName;
			}

			return null;
		}


		#region GetPersonsDisplayed

		private static List<Person> GetPersonsDisplayed(PersonFieldElement element, Reference reference)
		{
			if (element == null) return null;
			if (reference == null) return null;

			ReferencePersonCollection personCollection = reference.GetValue(element.PropertyId) as ReferencePersonCollection;
			if (personCollection == null || personCollection.Count == 0) return null;

			List<Person> persons = new List<Person>(personCollection);
			return persons;
		}

		private static List<Person> GetPersonsDisplayed(ComponentPart componentPart, Reference reference)
		{
			if (reference == null) return null;
			if (componentPart == null) return null;

			//check for 1st PersonFieldElement in ComponentPart
			PersonFieldElement firstPersonFieldElement = componentPart.Elements.FirstOrDefault(item => item is PersonFieldElement) as PersonFieldElement;
			if (firstPersonFieldElement == null) return null;

			return GetPersonsDisplayed(firstPersonFieldElement, reference);
		}


		private static List<Person> GetPersonsDisplayed(Template template, Reference reference)
		{
			if (reference == null) return null;
			if (template == null) return null;
			if (template.ComponentParts == null || template.ComponentParts.Count == 0) return null;

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
	}
}