//C6#BSO007
//Description:  Sort by first author and number of co-authors
//				1. Papers by a single author are listed chronologically from oldest to most recent.
//				2. Then papers by that author and a second, alphabetically according to the second author.
//				3. Then papers by that author and two co-authors, alphabetically according to the second author and the third author.
//				Alleiniger Autor, zwei Autoren, drei Autoren, vier Autoren, etc.; jeweils chronologisch vom ältesten zum neuesten Titel sortiert
//				1. Gruppe - 1 Autor:				Alleiniger Autor, Jahr (aufsteigend/ascending), Titel
//				2. Gruppe - 2 Autoren:				Erst-Autor, Zweit-Autor, Jahr (aufsteigend/ascending), Titel
//				3. Gruppe - 3 Autoren:				Erst-Autor, Zweit-Autor, Dritt-Autor, Jahr (aufsteigend/ascending), Titel
//				4. Gruppe - 4 Autoren:				Erst-Autor, Zweit-Autor, Dritt-Autor, Viert-Autor Jahr (aufsteigend/ascending), Titel
//				etc.
//Version 1.0 	

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
        CultureInfo _cultureForSorting = new CultureInfo("de");

        static readonly string[] _particlesToIgnore = new string[] { "a", "an", "the" };
        readonly Regex _wordsRegex = new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*");
        readonly Regex _particlesRegEx = new Regex(@"\b(" + string.Join("|", _particlesToIgnore) + @")\b", RegexOptions.IgnoreCase);

		
        public int Compare(Citation x, Citation y)
        {
            /*
				This is an example of a custom sort macro that sorts all references of type 'internet document' on top of the bibliography.
				The internet documents themselves are sorted according to a different logic than the rest of the cited documents.
				Return values:
				0:					x is considered the same as y sorting-wise, so we cannot tell a difference based on the algorithm below
				> 0 (positive):		x should go after y, x is greater than y
				< 0 (negative):		x should go before y, x is less than
			*/


            if (x == null || y == null) return 0;

            Reference xReference = x.Reference;
            Reference yReference = y.Reference;
            if (xReference == null || yReference == null) return 0;

            ReferenceType xReferenceType = xReference.ReferenceType;
            ReferenceType yReferenceType = yReference.ReferenceType;

            var xAuthors = xReference.AuthorsOrEditorsOrOrganizations;
            var yAuthors = yReference.AuthorsOrEditorsOrOrganizations;

            int xAuthorsCount = 0;
            int yAuthorsCount = 0;
            if (xAuthors != null) xAuthorsCount = xAuthors.Count;
            if (yAuthors != null) yAuthorsCount = yAuthors.Count;

            string xTitleForSorting = GetTitleForSorting(xReference);
            string yTitleForSorting = GetTitleForSorting(yReference);

            string xVolume = xReference.Volume;
            string yVolume = yReference.Volume;

            string xSeriesTitleForSorting = GetSeriesTitleForSorting(xReference);
            string ySeriesTitleForSorting = GetSeriesTitleForSorting(yReference);


            StringComparer defaultStringComparer = StringComparer.Create(_cultureForSorting, true);
            int authorCompareResult = 0;
            int authorsCountCompareResult = 0;
            int titleCompareResult = 0;
            int seriesTitleCompareResult = 0;
            int volumeCompareResult = 0;
            int yearCompareResult = 0;
            int editionCompareResult = 0;

            /*
				Die Werke werden in alphabetischer Reihenfolge nach den Familiennamen der Erstautoren bzw. -autorinnen gereiht.
				Ist bei einer Quelle kein Autor bzw. keine Autorin vorhanden, rückt der Titel an die Stelle des Autorennamens und das Werk
				wird nach dem ersten Wort des Titels (wobei bestimmte und unbestimmte Artikel unberücksichtigt bleiben) alphabetisch gereiht.
			*/


            if (xAuthorsCount == 0 && yAuthorsCount == 0)
            {
                //compare Titles
                titleCompareResult = xTitleForSorting.CompareTo(yTitleForSorting);
                if (titleCompareResult != 0) return titleCompareResult;

                seriesTitleCompareResult = xSeriesTitleForSorting.CompareTo(ySeriesTitleForSorting);
                if (seriesTitleCompareResult != 0) return seriesTitleCompareResult;
            }
            else if (xAuthorsCount == 0 && yAuthorsCount > 0)
            {
                //compare xTitle with yFirstAuthor
                titleCompareResult = defaultStringComparer.Compare(xTitleForSorting, yAuthors.ElementAt(0).FullName);
                if (titleCompareResult != 0) return titleCompareResult;
            }
            else if (xAuthorsCount > 0 && yAuthorsCount == 0)
            {
                //compare xFirstAuthor with yTitle
                titleCompareResult = defaultStringComparer.Compare(xAuthors.ElementAt(0).FullName, yTitleForSorting);
                if (titleCompareResult != 0) return titleCompareResult;
            }
            else
            {
                int minAuthorCount = Math.Min(xAuthorsCount, yAuthorsCount);

                for (int i = 0; i < minAuthorCount; i++)
                {
                    authorCompareResult = CompareAuthors(xAuthors.ElementAt(i), yAuthors.ElementAt(i));
                    if (authorCompareResult != 0) return authorCompareResult;

                    authorsCountCompareResult = xAuthorsCount.CompareTo(yAuthorsCount);
                    if (authorsCountCompareResult != 0) return authorsCountCompareResult;
                }
            }


            /*
				Innerhalb der Gruppe chronologisch aufsteigend
			*/
            #region Year

            yearCompareResult = YearComparer.Compare(x, y);
            if (yearCompareResult != 0) return yearCompareResult;

            #endregion Year

            #region Volume

            if
            (
                xReferenceType == yReferenceType &&
                xReference.HasCoreField(ReferenceTypeCoreFieldId.Volume) &&
                yReference.HasCoreField(ReferenceTypeCoreFieldId.Volume)
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

            #region Title

            titleCompareResult = defaultStringComparer.Compare(xTitleForSorting, yTitleForSorting);
            if (titleCompareResult != 0) return titleCompareResult;


            #endregion Title

            #region Edition

            if
            (
                xReference.HasCoreField(ReferenceTypeCoreFieldId.Edition) &&
                yReference.HasCoreField(ReferenceTypeCoreFieldId.Edition)
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

        private int CompareAuthors(Person xAuthor, Person yAuthor)
        {
            if (xAuthor == null || yAuthor == null) return 0;

            StringComparer defaultStringComparer = StringComparer.Create(_cultureForSorting, true);

            var xLastNameForSortingResolved = xAuthor.GetLastNameForSortingResolved();
            var yLastNameForSortingResolved = yAuthor.GetLastNameForSortingResolved();

            //var lastNameCompareResult = xLastNameForSortingResolved.CompareTo(yLastNameForSortingResolved);
            var lastNameCompareResult = defaultStringComparer.Compare(xLastNameForSortingResolved, yLastNameForSortingResolved);
            if (lastNameCompareResult != 0) return lastNameCompareResult;

            if (_cultureForSorting == CultureInfo.GetCultureInfo("de-DE"))
            {
                var lengthCompareResult = xLastNameForSortingResolved.CompareTo(yLastNameForSortingResolved);
                if (lengthCompareResult != 0) return lengthCompareResult;
            }

            var xFirstAndMiddleName = ConcatNonEmptyStrings(" ", new string[] { xAuthor.FirstName, xAuthor.MiddleName, xAuthor.Prefix, xAuthor.Suffix });
            var yFirstAndMiddleName = ConcatNonEmptyStrings(" ", new string[] { xAuthor.FirstName, xAuthor.MiddleName, xAuthor.Prefix, xAuthor.Suffix });

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
	}
}
