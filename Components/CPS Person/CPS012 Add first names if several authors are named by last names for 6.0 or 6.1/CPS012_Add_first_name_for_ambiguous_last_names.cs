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
        //CPS012 Add first or middle names for ambiguous last names v3.5
		
		//IMPORTANT: in Citavi 6.1 or below, it is only possible to identify persons with the same last name, not first or middle names.
		//Also disambiguation can only take place in a single stp, e.g. by adding first name initials only, or by adding full first names, or by adding both first and middle names.
        public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
        {
			//IMPORTANT: It is not possible Citavi 6.1 or below, to confine the name ambiguity check to the primary authors only.
			//If this is a requirement, pls. update to Citavi 6.2 or higher and make use of the latest component macr CPS012 version 4.0 or higher.
			//It is possible though to confine the disambiguation to the primary authors.
			bool disambiguatePrimaryAuthorsOnly = true;
			
			//When a first and/or middle name is added for disambiguation, should that be before or after the last name?
            PersonNameOrder nameOrderForAmbiguityResolution = PersonNameOrder.FirstNameLastName;
			
			//In case of ambiguous last names, should the disambiguation happen by adding full first names or just the initials?
            NameFormat firstNameFormatForAmbiguityResolution = NameFormat.Abbreviated; 
			//NameFormat.Full					John Mike
			//NameFormat.Abbreviated			J. M.
			//NameFormat.AbbreviatedNoPeriod	J M
			//NameFormat.AbbreviatedCompact		J.M.
			//NameFormat.Compact				JM
			
			//Should the middle name(s) be added for disambiguation?
			MiddleNameUsage middleNameUsageForAmbiguityResolution = MiddleNameUsage.All;
			//MiddleNameUsage.FirstOnly
			//MiddleNameUsage.All
			
			//In case of ambiguous last names, should disambiguation happen by adding full middle names or just the initials?
			NameFormat middleNameFormatForAmbiguityResolution = NameFormat.Full;
			
			
            handled = false;

            if (citation == null || citation.Reference == null) return null;
            if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			
            CitationManager citationManager = citation.CitationManager;
            if (citationManager == null) return null;

            PersonFieldElement personFieldElement = componentPart.Elements.OfType<PersonFieldElement>().FirstOrDefault();
            if (personFieldElement == null) return null;
            if (personFieldElement.SuppressOutput) return null;
				
            #region BeforeFormatPerson: Resolve last name ambiguity

            BeforeFormatPersonEventArgs bfp;
            personFieldElement.PersonFormatter.BeforeFormatPerson +=
            (sender, e) =>
            {
                bfp = (BeforeFormatPersonEventArgs)e;
                if (bfp.Person == null) return;
                if (disambiguatePrimaryAuthorsOnly && bfp.Index > 1) return;
				
                bool isLastNameAmbiguous = citationManager.IsCitedPersonLastNameAmbiguous(bfp.Person.LastName);
                if (!isLastNameAmbiguous) return;
				
				bfp.FirstNameFormat = firstNameFormatForAmbiguityResolution;
				bfp.NameOrder = nameOrderForAmbiguityResolution;
				bfp.MiddleNameUsage = middleNameUsageForAmbiguityResolution;
				bfp.MiddleNameFormat = middleNameFormatForAmbiguityResolution;
            };

            #endregion

            return null;
        }	
    }
}
