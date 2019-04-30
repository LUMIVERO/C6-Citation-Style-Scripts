//C6#CPS020
//Abbreviate name prefixes in the footnotes and ignore for sorting in the bibliography - Namenspräfix auch bei 'non-dropping particle' abkürzen
//Version: 1.0 - for Citavi 6 (or higher)

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
			
			if (!personFieldElement.AbbreviateNamePrefixes || string.IsNullOrEmpty(personFieldElement.NamePrefixAbbreviations)) return null;
			
			Dictionary<string, string> namePrefixAbbreviationsDictionary = GetNamePrefixAbbreviationsDictionary(personFieldElement.NamePrefixAbbreviations);
			

			#region BeforeFormatPerson
			
			BeforeFormatPersonEventArgs b;
			personFieldElement.PersonFormatter.BeforeFormatPerson += 
			(sender, e) =>
			{
				b = (BeforeFormatPersonEventArgs)e;
				if (b.Person == null || 
					string.IsNullOrEmpty(b.Person.LastName) ||
					!string.IsNullOrEmpty(b.Person.Prefix)) return;
				
				bool found = false;
				string oldLastName = b.Person.LastName;
				string newLastName = null;
				
				foreach(KeyValuePair<string, string> kvp in namePrefixAbbreviationsDictionary)
				{
					if (oldLastName.StartsWith(kvp.Key + " ", System.StringComparison.OrdinalIgnoreCase))
					{
						found = true;
						newLastName = kvp.Value + oldLastName.Substring(kvp.Key.Length);
					}
				}
				if (!found) return;
				
				var newPerson = new Person(b.Person.Project);
				newPerson.LastName = newLastName;
				newPerson.FirstName = b.Person.FirstName;
				newPerson.MiddleName = b.Person.MiddleName;
				newPerson.Prefix = b.Person.Prefix;
				newPerson.Suffix = b.Person.Suffix;
				newPerson.Abbreviation = b.Person.Abbreviation;
				
				b.Person = newPerson;
			};
			
			#endregion
			
			return null;
		}
		
		Dictionary<string, string> GetNamePrefixAbbreviationsDictionary(string input)
		{
			string key = null;
			Dictionary<string, string> namePrefixAbbreviationsDictionary = new Dictionary<string, string>();
			
			string[] namePrefixAbbreviations = input.Split(new char[] { '|' });
			for (int i = 0; i < namePrefixAbbreviations.Length; i++)
			{
				key = namePrefixAbbreviations[i].ToLower().Trim();
				if (!namePrefixAbbreviationsDictionary.ContainsKey(namePrefixAbbreviations[i]))
				{
					if (i == namePrefixAbbreviations.Length - 1)
						namePrefixAbbreviationsDictionary.Add(key, string.Empty);
					else
						namePrefixAbbreviationsDictionary.Add(key, namePrefixAbbreviations[i + 1].Trim());
				}
				i++;
			}
			return namePrefixAbbreviationsDictionary;
		}
	}
}
