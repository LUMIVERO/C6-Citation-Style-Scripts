//C6#BSO006
//C5#43311
//Description:  Sort by first author, author with 1 co-author and author with multiple co-authors
//				1. Papers by a single author are listed chronologically from oldest to most recent.
//				2. Then papers by that author and a second, alphabetically according to the second author.
//				3. Papers by three or more authors are listed chronologically.
//				Alleiniger Autor, zwei Autoren, mehrere Autoren; chronologisch vom ältesten zum neuesten Titel sortiert - Springer-Sortierung
//				1. Gruppe - 1 Autor:				Alleiniger Autor, Jahr (aufsteigend/ascending), Titel
//				2. Gruppe - 2 Autoren:				Erst-Autor, Zweit-Autor, Jahr (aufsteigend/ascending), Titel
//				3. Gruppe - 3 und mehr Autoren:		Erst-Autor, Jahr (aufsteigend/ascending), Titel - Die Ko-Autoren werden bei der Sortierung also ignoriert!
//													Diese Sortierung für die 3. Gruppe korrespondiert anscheinend mit dem Kurznachweis im Text, wo ab 3 Autoren mit Erst-Autor et al. abgekürzt wird.
//Version 1.2   Slight improvements regarding sorting
//Version 1.1 	Position of first person fieldelement does not matter anymore

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Citations;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SwissAcademic.Citavi.Comparers
{
   public class CustomCitationComparer
      :
      ICustomCitationComparerMacro
   {
 		CultureInfo _cultureForSorting = new CultureInfo("en");

        static readonly string[] _particlesToIgnore = new string[] { "a", "an", "the" };
        readonly Regex _wordsRegex = new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*");
        readonly Regex _particlesRegEx = new Regex(@"\b(" + string.Join("|", _particlesToIgnore) + @")\b", RegexOptions.IgnoreCase);


        public int Compare(Citation x, Citation y)
        {
            /*
				This is an example of a custom sort macro that sorts all references of type 'internet document' on top of the bibliography.
				The internet documents themselves are sorted according to a different logic than the rest of the cited documents.
				Return values:
				0:               x is considered the same as y sorting-wise, so we cannot tell a difference based on the algorithm below
				> 0 (positive):      x should go after y, x is greater than y
				< 0 (negative):      x should go before y, x is less than
			*/


            if (x == null || y == null) return 0;

            Reference xReference = x.Reference;
            Reference yReference = y.Reference;
            if (xReference == null || yReference == null) return 0;

            ReferenceType xReferenceType = xReference.ReferenceType;
            ReferenceType yReferenceType = yReference.ReferenceType;

            BibliographyCitation xBibliographyCitation = x as BibliographyCitation;
            BibliographyCitation yBibliographyCitation = y as BibliographyCitation;
            if (xBibliographyCitation == null || yBibliographyCitation == null) return 0;


            Template xTemplate = x.GetTemplateForCitation();
            Template yTemplate = y.GetTemplateForCitation();
            if (xTemplate == null || yTemplate == null) return 0;

            PersonFieldElement xPersonFieldElement = xTemplate.StructuralPersonFieldElement;
            PersonFieldElement yPersonFieldElement = yTemplate.StructuralPersonFieldElement;

            IEnumerable<Person> xPersons = xPersonFieldElement != null ? xPersonFieldElement.GetPersonsCited(x) : Enumerable.Empty<Person>();
            IEnumerable<Person> yPersons = yPersonFieldElement != null ? yPersonFieldElement.GetPersonsCited(y) : Enumerable.Empty<Person>();

            int xPersonsCount = 0;
            int yPersonsCount = 0;
            if (xPersons != null) xPersonsCount = xPersons.Count();
            if (yPersons != null) yPersonsCount = yPersons.Count();

            string xTitleForSorting = GetTitleForSorting(xReference);
            string yTitleForSorting = GetTitleForSorting(yReference);

            string xVolume = xReference.Volume;
            string yVolume = yReference.Volume;

            string xSeriesTitleForSorting = GetSeriesTitleForSorting(xReference);
            string ySeriesTitleForSorting = GetSeriesTitleForSorting(yReference);

            string xNumber = xReference.Number;
            string yNumber = yReference.Number;

            StringComparer defaultStringComparer = StringComparer.Create(_cultureForSorting, true);
            int personsCompareResult = 0;
            int personsCountCompareResult = 0;
            int titleCompareResult = 0;
            int seriesTitleCompareResult = 0;
            int volumeCompareResult = 0;
            int numberCompareResult = 0;
            int yearCompareResult = 0;
            int editionCompareResult = 0;


            if (xPersonsCount == 0 && yPersonsCount == 0)
            {
                //compare Titles
                titleCompareResult = xTitleForSorting.CompareTo(yTitleForSorting);
                if (titleCompareResult != 0) return titleCompareResult;

                seriesTitleCompareResult = xSeriesTitleForSorting.CompareTo(ySeriesTitleForSorting);
                if (seriesTitleCompareResult != 0) return seriesTitleCompareResult;
            }
            else if (xPersonsCount == 0 && yPersonsCount > 0)
            {
                //compare xTitle with yFirstAuthor
                titleCompareResult = defaultStringComparer.Compare(xTitleForSorting, yPersons.ElementAt(0).FullName);
                if (titleCompareResult != 0) return titleCompareResult;
            }
            else if (xPersonsCount > 0 && yPersonsCount == 0)
            {
                //compare xFirstAuthor with yTitle
                titleCompareResult = defaultStringComparer.Compare(xPersons.ElementAt(0).FullName, yTitleForSorting);
                if (titleCompareResult != 0) return titleCompareResult;
            }
            else
            {
                /*
				1. Single Author Group
				2. Author + Single Co-Author Group
				3. Author + Multiple-Co-Authors Group
				*/

                //compare by first author
                personsCompareResult = ComparePersons(xPersons.ElementAt(0), yPersons.ElementAt(0));
                if (personsCompareResult != 0) return personsCompareResult;

                //still here? then both have same first author
                if ((xPersonsCount == 1 && yPersonsCount > 1) || (xPersonsCount == 2 && yPersonsCount > 2))
                {
                    //x before y
                    return -1;
                }
                if ((xPersonsCount > 1 && yPersonsCount == 1) || (xPersonsCount > 2 && yPersonsCount == 2))
                {
                    //x after y
                    return 1;
                }
                if (xPersonsCount == 2 && yPersonsCount == 2)
                {
                    //compare by co-author
                    personsCompareResult = ComparePersons(xPersons.ElementAt(1), yPersons.ElementAt(1));
                    if (personsCompareResult != 0) return personsCompareResult;
                }

                //still here? 
                //both have either exactly one identical author OR
                //both have exactly two identical authors OR
                //more than 2 authors with identical first author

                //we continue with year comparison
            }


            #region Year

            yearCompareResult = YearComparer.Compare(x, y);
            if (yearCompareResult != 0) return yearCompareResult;

            #endregion Year

            #region Title

            titleCompareResult = defaultStringComparer.Compare(xTitleForSorting, yTitleForSorting);
            if (titleCompareResult != 0) return titleCompareResult;


            #endregion Title

            #region Volume

            if
            (
                xReferenceType == yReferenceType &&
                xReference.HasCoreField(ReferenceTypeCoreFieldId.Volume) &&
                yReference.HasCoreField(ReferenceTypeCoreFieldId.Volume) && 
                HasFieldElement(xTemplate, ReferencePropertyId.Volume) &&
                HasFieldElement(yTemplate, ReferencePropertyId.Volume)
            )
            {
                NumberStringComparer volumeComparer = new NumberStringComparer()
                {
                    CompareMode = NumberStringCompareMode.ByTextAndNumbersSegmentwise,
                    UseAbsoluteNumbersOnly = true
                };

                volumeCompareResult = volumeComparer.Compare(xVolume, yVolume);
                if (volumeCompareResult != 0) return volumeCompareResult;
            }

            #endregion Volume

            #region Number

            if 
            (
                xReferenceType == yReferenceType &&
                xReference.HasCoreField(ReferenceTypeCoreFieldId.Number) && 
                yReference.HasCoreField(ReferenceTypeCoreFieldId.Number) &&
                HasFieldElement(xTemplate, ReferencePropertyId.Number) &&
                HasFieldElement(yTemplate, ReferencePropertyId.Number)
            )
            {
                NumberStringComparer numberComparer = new NumberStringComparer()
                {
                    CompareMode = NumberStringCompareMode.ByTextAndNumbersSegmentwise,
                    UseAbsoluteNumbersOnly = true
                };

                numberCompareResult = numberComparer.Compare(xNumber, yNumber);
                if (numberCompareResult != 0) return numberCompareResult;
            }

            #endregion

            #region Edition

            if
            (
                xReference.HasCoreField(ReferenceTypeCoreFieldId.Edition) &&
                yReference.HasCoreField(ReferenceTypeCoreFieldId.Edition) &&
                HasFieldElement(xTemplate, ReferencePropertyId.Edition) &&
                HasFieldElement(yTemplate, ReferencePropertyId.Edition)
            )
            {
                var xEdition = xReference.EditionNumberResolved;
                var yEdition = yReference.EditionNumberResolved;

                bool xHasEdition = !string.IsNullOrEmpty(xEdition);
                bool yHasEdition = !string.IsNullOrEmpty(yEdition);


                if (xHasEdition && yHasEdition)
                {
                    NumberStringComparer editionComparer = new NumberStringComparer()
                    {
                        CompareMode = NumberStringCompareMode.ByTextAndNumbersSegmentwise,
                        UseAbsoluteNumbersOnly = true
                    };

                    editionCompareResult = editionComparer.Compare(xEdition, yEdition);
                    if (editionCompareResult != 0) return editionCompareResult;
                }
                else if (xHasEdition && !yHasEdition) //y before x
                {
                    return 1;
                }
                else if (!xHasEdition && yHasEdition) //x before y
                {
                    return -1;
                }
            }

            #endregion

            return 0;

        }

        private int ComparePersons(Person xPerson, Person yPerson)
        {
            if (xPerson == null || yPerson == null) return 0;

            StringComparer defaultStringComparer = StringComparer.Create(_cultureForSorting, true);

            var xLastNameForSortingResolved = xPerson.GetLastNameForSortingResolved();
            var yLastNameForSortingResolved = yPerson.GetLastNameForSortingResolved();

            //var lastNameCompareResult = xLastNameForSortingResolved.CompareTo(yLastNameForSortingResolved);
            var lastNameCompareResult = defaultStringComparer.Compare(xLastNameForSortingResolved, yLastNameForSortingResolved);
            if (lastNameCompareResult != 0) return lastNameCompareResult;

            if (_cultureForSorting == CultureInfo.GetCultureInfo("de-DE"))
            {
                var lengthCompareResult = xLastNameForSortingResolved.CompareTo(yLastNameForSortingResolved);
                if (lengthCompareResult != 0) return lengthCompareResult;
            }

            var xFirstAndMiddleName = ConcatNonEmptyStrings(" ", new string[] { xPerson.FirstName, xPerson.MiddleName, xPerson.Prefix, xPerson.Suffix });
            var yFirstAndMiddleName = ConcatNonEmptyStrings(" ", new string[] { yPerson.FirstName, yPerson.MiddleName, yPerson.Prefix, yPerson.Suffix });

            var firstAndMiddleNameCompareResult = defaultStringComparer.Compare(xFirstAndMiddleName, yFirstAndMiddleName);
            if (firstAndMiddleNameCompareResult != 0) return firstAndMiddleNameCompareResult;

            if (_cultureForSorting == CultureInfo.GetCultureInfo("de-DE"))
            {
                var lengthCompareResult = xFirstAndMiddleName.Length.CompareTo(yFirstAndMiddleName.Length);
                if (lengthCompareResult != 0) return lengthCompareResult;
            }

            return 0;


        }

        private CitationComparer YearComparer
        {
            get
            {
                var yearSortDescriptors = new List<PropertySortDescriptor<Reference>>();
                yearSortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.YearResolved, ListSortDirection.Ascending));

                var yearComparer = new CitationComparer(yearSortDescriptors);
                return yearComparer;
            }
        }

        #region ConcatNonEmptyStrings

        private string ConcatNonEmptyStrings(string separator, IEnumerable<string> strings)
        {
            if (strings == null) return string.Empty;
            if (strings.Count() == 0) return string.Empty;

            string fullString = string.Empty;
            for (int i = 0; i < strings.Count(); i++)
            {
                string stringToAdd = strings.ElementAt(i).Trim();

                if (string.IsNullOrEmpty(stringToAdd)) continue;
                if (string.IsNullOrEmpty(fullString))
                {
                    fullString = stringToAdd;
                }
                else
                {
                    fullString = string.Join(separator, new string[] { fullString, stringToAdd });
                }
            }

            return fullString;
        }

        #endregion ConcatNonEmptyStrings

        #region GetTitleForSorting

        private string GetTitleForSorting(Reference reference)
        {
            if (reference == null) return string.Empty;

            //change CustomField1 zu some other CustomFieldN where the sorting title can be found or
            //place two slashes in front of the following line to NOT make use of a special sorting title field at all
            //if (!string.IsNullOrEmpty(reference.CustomField1)) return reference.CustomField1;

            //still here? Then the sort title is derived from the title

            string sortTitle = ConcatNonEmptyStrings(" ", new string[] { reference.Title, reference.Subtitle, reference.TitleSupplement });
            return GetTextForSorting(sortTitle);
        }

        private string GetSeriesTitleForSorting(Reference reference)
        {
            if (reference == null) return string.Empty;

            if (!reference.HasCoreField(ReferenceTypeCoreFieldId.SeriesTitle)) return string.Empty;
            if (reference.SeriesTitle == null) return string.Empty;

            string sortSeriesTitle = reference.SeriesTitle.FullName;
            if (string.IsNullOrEmpty(sortSeriesTitle)) return string.Empty;

            return GetTextForSorting(sortSeriesTitle);
        }

        private string GetTextForSorting(string text)
        {
            if (_particlesToIgnore == null || !_particlesToIgnore.Any()) return text;
            if (_particlesRegEx == null) return text;

            var words = _wordsRegex.Split(text);
            int firstNonParticleToIgnoreIndex = -1;

            foreach (string word in words)
            {

                if (string.IsNullOrEmpty(word)) break;

                firstNonParticleToIgnoreIndex++;
                if (!_particlesRegEx.IsMatch(word)) break;
            }

            if (firstNonParticleToIgnoreIndex == -1) return text;       //either text is empty or consists of particles to ignore ONLY
            if (firstNonParticleToIgnoreIndex == 0) return text;        //text starts with a relevant word

            return string.Join(" ", words, firstNonParticleToIgnoreIndex, words.Length - firstNonParticleToIgnoreIndex);
        }

        #endregion GetTitleSorting

        #region HasFieldElement

        private bool HasFieldElement(Template template, ReferencePropertyId propertyId)
        {
            if (template == null) return false;
            if (propertyId == ReferencePropertyId.None) return false;
            foreach (var element in template.Elements)
            {
                if (element is FieldElement && ((FieldElement)element).PropertyId == propertyId) return true;
            }
            return false;
        }

        #endregion
		
	}
}
