//CPS024 Suppress spaces in Chinese names

using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using SwissAcademic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;
using SwissAcademic.Globalization;

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
            if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;

            PersonFieldElement personFieldElement = componentPart.Elements.OfType<PersonFieldElement>().FirstOrDefault();
            if (personFieldElement == null) return null;
            if (personFieldElement.SuppressOutput) return null;
			
			#region BeforeFormatPerson:

            BeforeFormatPersonEventArgs bfp;
            personFieldElement.PersonFormatter.BeforeFormatPerson +=
            (sender, e) =>
            {
                bfp = (BeforeFormatPersonEventArgs)e;
				if (bfp.Person == null) return;
				if (string.IsNullOrEmpty(bfp.Person.FullName)) return;
				
				if (bfp.Person.FullName.Any(c => IsChinese(c)))
				{
					// if required, Chinese names can have a different name order than other persons in the group
					// otherwise, pls. comment out the following line using to slashes //
					bfp.NameOrder = PersonNameOrder.FirstNameLastName;
					
					bfp.SpaceBetwenLastAndFirstnames = ""; 		// used for PersonNameOrder = FirstNameLastName
					bfp.LastNameFirstNameSeparator = "";		// used for PersonNameOrder = LastNameFirstName or LastNameFirstNameCompact;
				}
            };

            #endregion
			
			return null;
		}
		
		
		private static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
		static bool IsChinese(char c)
		{
		    return cjkCharRegex.IsMatch(c.ToString());
		}
	}
}
