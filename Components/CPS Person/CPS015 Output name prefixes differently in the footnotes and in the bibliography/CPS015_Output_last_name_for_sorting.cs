//C6#CPS015
//Description: Output last name for sorting of each person (if exists)
//Version: 2.0 - Completely re-written for Citavi 6 (6.0.1.6 Beta or higher)

using System.Linq;
using System.Collections.Generic;
namespace SwissAcademic.Citavi.Citations
{

	public class ComponentPartFilter 
		: 
		IComponentPartFilter
	{
		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			handled = false;

			if (citation == null || citation.Reference == null) return null;

			if (componentPart == null) return null;
			if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;

			PersonFieldElement personFieldElement = componentPart.Elements.OfType<PersonFieldElement>().FirstOrDefault();
			if (personFieldElement == null) return null;

			#region BeforeFormatPerson
			
			BeforeFormatPersonEventArgs b;
			personFieldElement.PersonFormatter.BeforeFormatPerson += 
			(sender, e) =>
			{
				b = (BeforeFormatPersonEventArgs)e;
				if (b.Person == null) return;
				if (string.IsNullOrEmpty(b.Person.LastNameForSorting)) return;
				
				var sortNamePerson = new Person(b.Person.Project, b.Person.LastNameForSorting);
				b.Person = sortNamePerson;
			};
			
			#endregion
			
			return null;
		}
	}
}
