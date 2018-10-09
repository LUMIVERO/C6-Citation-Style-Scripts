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
        //CPS012 Add first or middle names for ambiguous last names v4.0
		//Disambiguation of identical person names by successively adding first name initials, full first names, middle name initals and full middle names (if available)
        public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
        {
			//Should only the primary authors be considered in checking last names for ambiguity?
            bool checkAmbiguityForPrimaryAuthorsOnly = false;
			
			//When a first and/or middle name is added for disambiguation, should that be before or after the last name?
            PersonNameOrder nameOrderForAmbiguityResolution = PersonNameOrder.LastNameFirstName;
			
			//In case of ambiguous last names, should the first attempt to disambigutate be the addition of full first names or just the initials?
            NameFormat firstNameFormatForAmbiguityResolution = NameFormat.Abbreviated; 
			//NameFormat.Full					John Mike
			//NameFormat.Abbreviated			J. M.
			//NameFormat.AbbreviatedNoPeriod	J M
			//NameFormat.AbbreviatedCompact		J.M.
			//NameFormat.Compact				JM
			
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
                if (checkAmbiguityForPrimaryAuthorsOnly && bfp.Index > 1) return;

                bool isLastNameAmbiguous = checkAmbiguityForPrimaryAuthorsOnly ? 
					citationManager.IsFirstCitedPersonLastnameAmbiguous(bfp.Person.LastName) :
					citationManager.IsCitedPersonLastNameAmbiguous(bfp.Person.LastName);
				
                if (!isLastNameAmbiguous) return;
				
				NameIdentity nameIdentity = checkAmbiguityForPrimaryAuthorsOnly ? 
					citationManager.GetFirstPersonNameIdentity(bfp.Person) :
					citationManager.GetPersonNameIdentity(bfp.Person);
				
				/*														PERSON A				PERSON B
				NameIdentity.None										Müller, Gerd Jakob		Meier, Konrad Martin
				NameIdentity.LastName									Müller, Gerd Jakob		Müller, Konrad Martin
				NameIdentity.LastNameFirstNameInitial					Müller, Gerd Jakob		Müller, Gustav Martin
				NameIdentity.LastNameFirstNameFull						Müller, Gerd Jakob		Müller, Gerd Martin
				NameIdentity.LastNameFirstNameFullMiddleNameInitial		Müller, Gerd Jakob		Müller, Gerd Johann
				NameIdentity.LastNameFirstNameFullMiddleNameFull		Müller, Gerd Jakob		Müller, Gerd Jakob [der Ältere]
				*/
				switch (nameIdentity)
				{
					case NameIdentity.None:
					default:
					{
						return;
					}
					case NameIdentity.LastName:
					{
						bfp.FirstNameFormat = firstNameFormatForAmbiguityResolution;
						bfp.MiddleNameUsage = MiddleNameUsage.None;
						break;
					}
					case NameIdentity.LastNameFirstNameInitial:
					{
						bfp.FirstNameFormat = NameFormat.Full;
						bfp.MiddleNameUsage = MiddleNameUsage.None;
						break;
					}
					case NameIdentity.LastNameFirstNameFull:
					{
						bfp.FirstNameFormat = NameFormat.Full;
						bfp.MiddleNameFormat = firstNameFormatForAmbiguityResolution;
						bfp.MiddleNameUsage = MiddleNameUsage.All;
						break;
					}
					case NameIdentity.LastNameFirstNameFullMiddleNameInitial:
					case NameIdentity.LastNameFirstNameFullMiddleNameFull:
					{
						bfp.FirstNameFormat = NameFormat.Full;
						bfp.MiddleNameFormat = NameFormat.Full;
						bfp.MiddleNameUsage = MiddleNameUsage.All;
						break;
					}
				}
				
				bfp.NameOrder = nameOrderForAmbiguityResolution;
            };

            #endregion

            return null;
        }
    }
}