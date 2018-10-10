// CPS012
//Add first or middle names for ambiguous last names
//Version 3.5
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

        public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
        {
			//IMPORTANT: It is not possible Citavi 6.1 or below, to confine the name ambiguity check to the primary authors only.
			//If this is a requirement, pls. update to Citavi 6.2 or higher and make use of the latest component macr CPS012 version 4.0 or higher.
			//It is possible though to confine the disambiguation to the primary authors.
			bool disambiguatePrimaryAuthorsOnly = true;
			
			//When a first and/or middle name is added for disambiguation, should that be before or after the last name?
            PersonNameOrder nameOrderForAmbiguityResolution = PersonNameOrder.LastNameFirstName;
			
			//In case of ambiguous last names, should the disambiguation happen by adding full first names or just the initials?
            NameFormat firstNameFormatForAmbiguityResolution = NameFormat.Full; 
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
