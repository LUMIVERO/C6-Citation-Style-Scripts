//C6#COT008
//C5#431516
//Description: Convert output to lower case (ensure first character is upper case)
//Version: 2.5
//Name of filter: Convert output to lower case (ensure first character is upper case)
//Version 2.5: Introduced new parameter modeStrict
//Version 2.4: Corrected: quotation marks at start of field/text units will not prevent first word to be capitalized
//Version 2.3: Changed: var ensureEnglishIsReferenceLanguage = true;
//Version 2.2: Corrected: 'Print-as-stated' exceptions not working if exception is very first word of field
//Version 2.1: Corrected: Splitting by interpunctuation
//Version 2.0: Added possibility to define expressions (abbreviations, proper names etc.) that will be printed exactly as stated
//Version 1.6: Consider parent's Language field if this is a child reference and the child's Language field is empty
//Version 1.5: Added checking for null on GetTextUnitsUnfiltered() to avoid NullReferenceExceptions at runtime (which may lead to auto-deactivation of filter)
//Version 1.4: ToUpperFirstLetter() method now handles words completely in UPPERCASE and takes culture into consideration
//Version 1.3: more punctuation (! and ?) added
//Version 1.2: based on TitleCase filter, option to start again with uppercase letter after punctuation (. and :)


using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using SwissAcademic.Citavi;
using System;
using System.Linq;

namespace SwissAcademic.Citavi.Citations
{
    public class ComponentPartFilter
        :
        IComponentPartFilter
    {
        public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
        {


            var ensureEnglishIsReferenceLanguage = true;    //if set to false, the component part filter will ALWAYS capitalize, regardless of the reference's language
            var upperCaseAfterPunctuation = true;           //if set to false, everything but the very first word will be lower case
			var modeStrict = false;				//only applicable if ensureEnglishIsReferenceLanguage = true: 
												//if modeStrict = true, it will only capitalize references that have "en" or "eng" etc. in the language field
												//if modeStrict = false, it will also capitalize references that have an empty language field

            CultureInfo culture = CultureInfo.CurrentCulture;

            handled = false;

            if (citation == null) return null;
            if (citation.Reference == null) return null;
            if (componentPart == null) return null;
            if (template == null) return null;

            if (ensureEnglishIsReferenceLanguage)
            {
                string languageResolved = citation.Reference.Language;
                if (componentPart.Scope == ComponentPartScope.Reference)
                {
                    //if ComponentPartScope is Reference, language can come from Reference or ParentReference
                    if (string.IsNullOrEmpty(languageResolved) && citation.Reference.ParentReference != null)
                    {
                        languageResolved = citation.Reference.ParentReference.Language;
                    }
                    if (string.IsNullOrEmpty(languageResolved) && modeStrict) return null;
                }
                else
                {
                    //if ComponentPartScope is ParentReference, language MUST come from ParentReference
                    if (citation.Reference.ParentReference == null) return null;
                    languageResolved = citation.Reference.ParentReference.Language;
                }
                if (string.IsNullOrEmpty(languageResolved) && modeStrict) return null;

                if (!string.IsNullOrEmpty(languageResolved))
                {
                    var termsList = new string[] {
                        "en",
                        "eng",
                        "engl",
                        "English",
                        "Englisch"
                    };


                    var regEx = new Regex(@"\b(" + string.Join("|", termsList) + @")\b", RegexOptions.IgnoreCase);
                    if (!regEx.IsMatch(languageResolved))
                    {
                        return null;
                    }
                }
            }	//


            var textUnits = componentPart.GetTextUnitsUnfiltered(citation, template);
            if (textUnits == null || !textUnits.Any()) return null;

            //Expressions that must not be changed with regards to capitalization
            List<string> printAsStatedExpressions = new List<string>()
            {
                "US", "USA", "UK", "UN", "ZDF", "ARD", "GmbH", "WDR",
                "Microsoft", "Google",
                "Cologne", "London", "Paris", "Moscow",
                "Germany", "France", "Italy", "Russia", "Sweden",
                "United Nations", "United States of America", "European Union", "CEO", "CSR", "VC","VCs", "American", "CEOs"
            };
            printAsStatedExpressions.Sort((x, y) => y.Length.CompareTo(x.Length)); //descending: longer ones first

            //Break the input text into a list of words at whitespaces,
            //hyphens, opening parens, and ASCII quotation marks
            //as well as the above printAsStatedExpressions
            string allInterpunctuation = @"(\s)|(-)|(\()|(\))|("")|(„)|(“)|(“)|(”)|(‘)|(’)|(«)|(»)|(\.)|(:)|(\?)|(!)";
            string splitPattern = printAsStatedExpressions.Count == 0 ?
                allInterpunctuation :
                string.Format(@"({0})", String.Join("|", printAsStatedExpressions.Select(x => string.Format(@"\b{0}\b", Regex.Escape(x))))) + "|" + allInterpunctuation;

            string interpunctuactionFollowedByCapitalization = @"(\.)|(:)|(\?)|(!)"; //next word will be capitalized if possible
            bool firstWordDone = false;

            for (int i = 0; i < textUnits.Count; i++)
            {
                //textUnit.Text = textUnits[i].Text.ToLower(culture);
                var text = textUnits[i].Text;

                List<string> words = Regex.Split(text, splitPattern, RegexOptions.IgnoreCase).Where(x => !string.IsNullOrEmpty(x)).ToList();

                text = string.Empty;

                for (int j = 0; j < words.Count; j++)
                {
                    var word = words[j].ToString();

                    if (Regex.IsMatch(word, allInterpunctuation) || word.Equals(" "))
                    {
                        //space or punctuation
                        text = text + word;
                        continue;
                    }

                    string printAsStatedExpression = printAsStatedExpressions.FirstOrDefault(ex => ex.Equals(word, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(printAsStatedExpression))
                    {
                        text = text + printAsStatedExpression;
                        firstWordDone = true;
                        continue;
                    }

                    if (((i == 0) && (j == 0)) || !firstWordDone)
                    {
                        text = text + ToUpperFirstLetter(word, culture);
                        firstWordDone = true;
                    }
                    else if (upperCaseAfterPunctuation && ((j > 0 && Regex.IsMatch(words[j - 1], interpunctuactionFollowedByCapitalization)) || (j > 1 && Regex.IsMatch(words[j - 2], interpunctuactionFollowedByCapitalization))))
                    {
                        text = text + ToUpperFirstLetter(word, culture);
                        firstWordDone = true;
                    }
                    else
                    {
                        text = text + word.ToLower(culture);
                        firstWordDone = true;
                    }
                }
                textUnits[i].Text = text;
            }

            handled = true;
            return textUnits;
        }

        public string ToUpperFirstLetter(string input, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(input)) return input;

            char[] letters = input.ToCharArray();

            for (var i = 0; i < letters.Length; i++)
            {
                if (i == 0)
                {
                    letters[0] = char.ToUpper(letters[0], culture);
                }
                else
                {
                    letters[i] = char.ToLower(letters[i], culture);
                }
            }

            return new string(letters);
        }
    }
}
