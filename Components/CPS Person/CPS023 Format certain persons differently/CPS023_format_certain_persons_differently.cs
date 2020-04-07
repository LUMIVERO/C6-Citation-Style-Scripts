using System;
using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;
using System.Reflection;

namespace SwissAcademic.Citavi.Citations
{
    public class ComponentPartFilter
        :
        IComponentPartFilter
    {
        //CPS023 Format certain persons differently, by adding an "f" to the beginning of the person's notes field
        public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
        {
			//When a first and/or middle name is added for disambiguation, should that be before or after the last name?
            PersonNameOrder nameOrder = PersonNameOrder.FirstNameLastName;
            NameFormat firstNameFormat = NameFormat.Abbreviated;
			FontStyle lastNameFontStyle = FontStyle.Neutral;
			FontStyle firstNameFontStyle = FontStyle.Italic;

			//NameFormat.Full					John Mike
			//NameFormat.Abbreviated			J. M.
			//NameFormat.AbbreviatedNoPeriod	J M
			//NameFormat.AbbreviatedCompact		J.M.
			//NameFormat.Compact				JM
			

			
            handled = false;

            if (citation == null || citation.Reference == null) return null;
            if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;

            PersonFieldElement personFieldElement = componentPart.Elements.OfType<PersonFieldElement>().FirstOrDefault();
            if (personFieldElement == null) return null;
            if (personFieldElement.SuppressOutput) return null;
			
			
            #region BeforeFormatPerson

            BeforeFormatPersonEventArgs bfp;
            personFieldElement.PersonFormatter.BeforeFormatPerson +=
            (sender, e) =>
            {
                bfp = (BeforeFormatPersonEventArgs)e;
                if (bfp.Person == null) return;
				if (string.IsNullOrWhiteSpace(bfp.Person.Notes)) return;
				
				//add ab "F" to the person's notes field to indicate: output with first name
				if (!bfp.Person.Notes.StartsWith("F", StringComparison.OrdinalIgnoreCase)) return;
				
				bfp.FirstNameFormat = firstNameFormat;
				bfp.NameOrder = nameOrder;
				bfp.FirstAndMiddleNameFontStyle = firstNameFontStyle;
				bfp.LastNameFontStyle = lastNameFontStyle;
            };

            #endregion

            return null;
        }
    }
}
