//CPS021 In the bibliography - Replace names after first mention by dashes

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
        public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
        {
            //Name of filter: Outputs a dash "---" instead of names in case of repetition 
			//Version 3.3: Corrected bug that organizations were not considered because of missing AfterFormatOrganization event handler
            //Version 3.2: Refactored for C6 to make use of AfterFormatPerson event handler
            //Version 3.1:
            //- set font style of dashes to neutral
            //Version 3.0:
            //- allow either one dash if all persons are the same (lookForRepetitionOfIndividualPersons = false) or various dashes for each repeated person (lookForRepetitionOfIndividualPersons = true)
            //- allow mix of dashes and written names and again dashes (exitLookForRepetitionIfFailedOnce = false) or not (exitLookForRepetitionIfFailedOnce = true)
            //Version 2.2: 
            //- GetPreviousVisibleCitation() method gets first previous citation where nobib = false
            //Version 2.1:
            //- filter deactivates itself, if the bibliography is NOT YET completely sorted (see below)
            //Version 2: 
            //- filter deactivates itself, if the person field component it is attached to is NOT the first inside the template
            //- filter works on ALL kinds of person fields (Authors, Editors, Organizations, AuthorsEditorsOrOrganizations etc.)
            //- filter compares ALL kinds of person fields of this citation with the ones of its predecessor (e.g. authors with authors, authors with editors etc.)
            //- filter considers group prefices for singular and plural already defined on the field element itself:
            //	--- (Hrsg.) or ---, eds. etc. I.e. it checks, if ", eds." has already been defined and uses it with the dash.
            //- you can customize the dash in line 34 below

            //NOTE: Set the following to true, if you want one dash if all persons are repeated, or none,
            //set it to false, if you want to compare person by person and set individual dashes per repeated person
            bool lookForRepetitionOfIndividualPersons = true;
            bool exitLookForRepetitionIfFailedOnce = true; //only applicable if previous was set to true

            /*
			 * lookForRepetitionOfIndividualPersons = FALSE:
			 * 
			 * A, B (2010)
			 * - (2010)
			 * A, B, C (2010)
			 * A, B, D (2010)
			 * A, B, C (2010)
			 * A, D, C (2010)
			 * 
			 * lookForRepetitionOfIndividualPersons = TRUE, exitLookForRepetitionIfFailedOnce = TRUE
			 * 
			 * A, B (2010)
			 * -, - (2010)
			 * A, B, C (2010)
			 * -, -, D (2010)
			 * A, B, C (2010)
			 * -, D, C (2010)
			 * 
			 * lookForRepetitionOfIndividualPersons = TRUE, exitLookForRepetitionIfFailedOnce = FALSE
			 * 
			 * A, B (2010)
			 * -, - (2010)
			 * A, B, C (2010)
			 * -, -, D (2010)
			 * A, B, C (2010)
			 * -, D, - (2010)
			 * 
			 */

            handled = false;

            if (citation == null || citation.Reference == null || citation.CitationManager == null) return null;
            if (template == null) return null;
            if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;

            //define the dashes
            string emdashes = "———";
            LiteralTextUnit emDashesTextUnit = new LiteralTextUnit(emdashes);

            //filter deactivates itself, if the bibliography is NOT YET completely sorted
            //this is necessary to avoid that this filter in turn changes the sort order, that it depends upon
            if (citation.CitationManager.BibliographyCitations.IsSorted == false) return null;

            //make sure the current componentPart is the FIRST inside the template
            if (template.ComponentParts == null || template.ComponentParts.Count == 0) return null;
            if (template.ComponentParts[0].Id != componentPart.Id) return null;

            #region ThisBibliographyCitation

            var thisBibliographyCitation = citation as BibliographyCitation;
            if (thisBibliographyCitation == null) return null;


            #endregion ThisBibliographyCitation

            #region PreviousBibliographyCitation

            var previousBibliographyCitation = GetPreviousVisibleBibliographyCitation(thisBibliographyCitation);
            if (previousBibliographyCitation == null) return null;
            if (previousBibliographyCitation.Reference == null) return null;
            if (previousBibliographyCitation.NoBib == true) return null;

            #endregion PreviousBibliographyCitation

            #region ThisTemplate

            var thisTemplate = thisBibliographyCitation.Template;
            if (thisTemplate == null) return null;

            #endregion ThisTemplate

            #region PreviousTemplate

            var previousTemplate = previousBibliographyCitation.Template;
            if (previousTemplate == null) return null;

            #endregion PreviousTemplate

            #region ThisPersonFieldElement

            var thisPersonFieldElement =
            (
                componentPart.Elements != null &&
                componentPart.Elements.Count > 0 ?
                componentPart.Elements[0] :
                null
            ) as PersonFieldElement;
            if (thisPersonFieldElement == null) return null;

            #endregion ThisPersonFieldElement

            #region PreviousPersonFieldElement

            var previousPersonFieldElement =
            (
                previousTemplate.ComponentParts != null &&
                previousTemplate.ComponentParts.Count > 0 &&
                previousTemplate.ComponentParts[0].Elements != null &&
                previousTemplate.ComponentParts[0].Elements.Count > 0 ?
                previousTemplate.ComponentParts[0].Elements[0] :
                null
            ) as PersonFieldElement;
            if (previousPersonFieldElement == null) return null;

            #endregion PreviousPersonFieldElement
			
            #region ThesePersons

            //we DO have a valid citation/reference a previous citation/reference, so we can compare their persons
            IEnumerable<Person> thesePersons = thisBibliographyCitation.Reference.GetValue(thisPersonFieldElement.PropertyId) as IEnumerable<Person>;
            if (thesePersons == null || thesePersons.Count() == 0) return null;

            #endregion ThesePersons

            #region PreviousPersons

            IEnumerable<Person> previousPersons = previousBibliographyCitation.Reference.GetValue(previousPersonFieldElement.PropertyId) as IEnumerable<Person>;
            if (previousPersons == null || previousPersons.Count() == 0) return null;

            bool failedOnce = false;

            #endregion PreviousPersons

            //we DO have authors in both cases to compare

            #region LookForRepetitionOfIndividualPersons = TRUE

            if (lookForRepetitionOfIndividualPersons)
            {
                /*
					A, B (2010)
					-, - (2010)
					A, B, C (2010)
					-, -, D (2010)
				*/

                AfterFormatPersonEventArgs afp;
				failedOnce = false;
                thisPersonFieldElement.PersonFormatter.AfterFormatPerson +=
                (sender, e) =>
                {
					#region AfterFormatPerson
					
                    if (exitLookForRepetitionIfFailedOnce && failedOnce) return;
                    afp = (AfterFormatPersonEventArgs)e;

                    Person thisPerson = afp.Person;
                    if (thisPerson == null)
                    {
                        failedOnce = true;
                        return;
                    }

                    Person previousPerson = previousPersons.ElementAtOrDefault(afp.Index);
                    if (previousPerson == null)
                    {
                        failedOnce = true;
                        return;
                    }


                    if (!thisPerson.Equals(previousPerson))
                    {
                        failedOnce = true;
                        return;
                    }

                    //same person
                    afp.TextUnits.Clear();
                    afp.TextUnits.Add(emDashesTextUnit);
					
					#endregion
                };
				
				AfterFormatOrganizationEventArgs afo;
				thisPersonFieldElement.PersonFormatter.AfterFormatOrganization += 
				(sender, e) =>
				{
					#region AfterFormatOrganization
					
					if (exitLookForRepetitionIfFailedOnce && failedOnce) return;
                    afo = (AfterFormatOrganizationEventArgs)e;

                    Person thisOrganization = afo.Organization;
                    if (thisOrganization == null)
                    {
                        failedOnce = true;
                        return;
                    }

                    Person previousOrganization = previousPersons.ElementAtOrDefault(afo.Index);
                    if (previousOrganization == null)
                    {
                        failedOnce = true;
                        return;
                    }


                    if (!thisOrganization.Equals(previousOrganization))
                    {
                        failedOnce = true;
                        return;
                    }

                    //same organization
                    afo.TextUnits.Clear();
                    afo.TextUnits.Add(emDashesTextUnit);
					
					#endregion
				};
            }

            #endregion LookForRepetitionOfIndividualPersons = TRUE

            #region LookForRepetitionOfIndividualPersons = FALSE

            else
            {
                if (!thesePersons.SequenceEqual(previousPersons)) return null;

                //check if there are group suffixe defined
                LiteralTextUnit thisGroupSuffixSingularTextUnit = thisPersonFieldElement.GroupSuffixSingular != null ?
                        new LiteralTextUnit(thisPersonFieldElement.GroupSuffixSingular) :
                        null;
                LiteralTextUnit thisGroupSuffixPluralTextUnit = thisPersonFieldElement.GroupSuffixPlural != null ?
                        new LiteralTextUnit(thisPersonFieldElement.GroupSuffixPlural) :
                        null;

                //we are dealing the the same author(s), so we handle the output now:
                TextUnitCollection output = new TextUnitCollection();
                if (thesePersons.Count() > 1 && thisGroupSuffixPluralTextUnit != null)
                {
                    output.Add(emDashesTextUnit);
                    output.Add(thisGroupSuffixSingularTextUnit);
                }
                else if (thesePersons.Count() == 1 && thisGroupSuffixSingularTextUnit != null)
                {
                    output.Add(emDashesTextUnit);
                    output.Add(thisGroupSuffixSingularTextUnit);
                }
                else
                {
                    output.Add(emDashesTextUnit);
                }

                //Set dashes to neutral font style
                foreach (ITextUnit unit in output)
                {
                    if (!unit.Text.Equals(emdashes)) continue;
                    unit.FontStyle = SwissAcademic.Drawing.FontStyle.Neutral;
                }

                handled = true;
                return output;
            }

            #endregion LookForRepetitionOfIndividualPersons = FALSE

            return null;
        }

        #region GetPreviousVisibleCitation

        private static BibliographyCitation GetPreviousVisibleBibliographyCitation(BibliographyCitation bibliographyCitation)
        {
            if (bibliographyCitation == null) return null;
            BibliographyCitation previousBibliographyCitation = bibliographyCitation;

            //consider nobib
            do
            {
                previousBibliographyCitation = previousBibliographyCitation.PreviousBibliographyCitation;
                if (previousBibliographyCitation == null) return null;

            } while (previousBibliographyCitation.NoBib == true);

            //still here? found one!
            return previousBibliographyCitation;
        }

        #endregion GetPreviousCitation
	}
}
