//C6#COT007
//C5#431516
//Description: Capitalize first letter of simple text field elements (such as Title or Subtitle etc.)
//Version 2.8: Corrected issue with text appearing twice under certain circumstances
//Version 2.7: Words of length 1 succeeded by a period "." are treated as initials and will be capitalized, see e.g. "The Complete Poems of A. R. Ammons"
//Version 2.6: Introduced new parameter modeStrict
//Version 2.5: Corrected handling of apostrophe for ommissions, plural handling or possessions, as in he's, don't, I'm, lov'd, p's and q's, comma's, Bernadette's but also Arabic, Japanese or Chinese words, like Shin'ichi etc.
//Version 2.4: Corrected handling of words in square brackets
//Version 2.3: Improved capitalization after punctuation
//Version 2.2: Slight improvements
//Version 2.1: Corrected handling of various typographical quotation marks
//Version 2.0: Corrected splitting by interpunctuation
//Version 1.9: Consider parent's Language field if this is a child reference and the child's Language field is empty
//Version 1.8: Parametrized conversion of full upper case words; functionality was impaired in version 1.7
//Version 1.7: Added checking for null on GetTextUnitsUnfiltered() to avoid NullReferenceExceptions at runtime (which may lead to auto-deactivation of filter)
//Version 1.6: ToUpperFirstLetter() method now handles words completely in UPPERCASE and takes culture into consideration
//Version 1.5: capitalize stopwords directly after quotation mark
//Version 1.4: improved word tokenization
//Version 1.3: ignore expressions which are written completely in upper case
//Version 1.2: new option to ensure that the reference language is "English"  

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
			var modeStrict = false;				//only applicable if ensureEnglishIsReferenceLanguage = true: 
												//if modeStrict = true, it will only capitalize references that have "en" or "eng" etc. in the language field
												//if modeStrict = false, it will also capitalize references that have an empty language field

            var convertFullUpperCaseWords = ConvertFullUpperCaseWords.Never;

            #region Info on ConvertFullUpperCaseWords parameter
            /*
				Example 1: UN and US government made agreement on payments of contribution
				Example 2: UN AND US GOVERNMENT MADE AGREEMENT ON PAYMENTS OF CONTRIBUTION
				ConvertFullUpperCaseWords.Never (default)
				Result 1: UN and US Government Made Agreement on Payments of Contribution
				Result 2: UN and US GOVERNMENT MADE AGREEMENT on PAYMENTS of CONTRIBUTION
				ConvertFullUpperCaseWords.Always: 
				Result 1: Un and Us Government Made Agreement on Payments of Contribution
				Result 2: Un and Us Government Made Agreement on Payments of Contribution
				ConvertFullUpperCaseWords.Auto:
				Result 1: UN and US Government Made Agreement on Payments of Contribution
				Result 2: Un and Us Government Made Agreement on Payments of Contribution
			*/
            #endregion

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
                if (string.IsNullOrEmpty(languageResolved) && modeStrict)
                {
                    return null;
                }


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
            }

            //Words that will not be capitalized; add words to this list as required
            string[] exceptionsArray = { "a", "an", "and", "as", "at",
                                         "but", "by", "down", "for", "from",
                                         "in", "into", "nor",
                                         "of", "on", "onto", "or", "over",
                                         "so", "the", "till", "to",
                                         "up", "via", "with", "yet" };

            List<string> exceptions = new List<string>(exceptionsArray);

            var textUnits = componentPart.GetTextUnitsUnfiltered(citation, template);
            if (textUnits == null || !textUnits.Any()) return null;

            string fullString = textUnits.ToString();
            bool fullUpperCaseTreatment = false;
            switch (convertFullUpperCaseWords)
            {
                case ConvertFullUpperCaseWords.Always:
                    fullUpperCaseTreatment = true;
                    break;

                case ConvertFullUpperCaseWords.Never:
                    {
                        fullUpperCaseTreatment = false;
                    }
                    break;

                default:
                case ConvertFullUpperCaseWords.Auto:
                    {
                        if (HasLowerCase(fullString))
                        {
                            fullUpperCaseTreatment = false;
                        }
                        else
                        {
                            fullUpperCaseTreatment = true;
                        }
                    }
                    break;
            }

            string prevWord = string.Empty;
            string secondPrevWord = string.Empty;
            string nextWord = string.Empty;

            List<string> words = null;
            List<string> nextWords = null;

            //Break the input text into a list of words at whitespaces,
            //hyphens, opening parens, and ASCII quotation marks
            string splitPattern = @"(\s)|(-)|(\()|(\))|(\[)|(\])|(\"")|(\')|(\u2018)|(\u2019)|(\u201A)|(\u201C)|(\u201D)|(\u201E)|(\u201F)|(\u2039)|(\u203A)|(\u00AB)|(\u00BB)|(\.)|(:)|(\?)|(!)|(\u2014)";

            string matchInterpunctuation = @"\.|:|\?|!|\u2014";
            string matchQuotationMarks = @"\""|\u2018|\u2019|\u201A|\u201C|\u201D|\u201E|\u201F|\u2039|\u203A|\u00AB|\u00BB";
            string matchApostrophe = @"'|\u2019"; //further 'FALSE' apostrophe characters: \u02bc, \u02c8, \u00b4, \u0060, \u2018, \u2032, \u02bb

            #region Infos about unicode characters used
            /*
             * \u0027  Apostrophe
             * \u005B  Left Square Bracket
             * \u005D  Right Square Bracket
             * \u2014  Geviertstrich
             * \u2018  Left Single Quotation Mark
             * \u2019  Right Single Quotation Mark
             * \u201A  Single Low-9 Quotation Mark
             * \u201C  Left Double Quotation Mark
             * \u201D  Right Double Quotation Mark
             * \u201E  Double Low-9 Quotation Mark
             * \u201F  Double High-Reversed-9 Quotation Mark
             * \u2039  Single Left-Pointing Angle Quotation Mark
             * \u203A  Single Right-Pointing Angle Quotation Mark
             * \u00AB  Double Left-Pointing Angle Quotation Mark
             * \u00BB  Double Right-Pointing Angle Quotation Mark
            */
            #endregion


            for (int i = 0; i < textUnits.Count; i++)
            {
                //textUnit.Text = textUnits[i].Text.ToLower(culture);
                var text = textUnits[i].Text;
                var nextText = i < textUnits.Count - 1 ? textUnits[i + 1].Text : null;

                words = i > 0 ? nextWords : new List<string>(Regex.Split(text, splitPattern).Where(s => s != string.Empty));
                nextWords = !string.IsNullOrEmpty(nextText) ?
                    new List<string>(Regex.Split(nextText, splitPattern).Where(s => s != string.Empty)) :
                    new List<string>();
                    

                var counter = 0;
                text = string.Empty;

                //Check each remaining word against the list, and append it to the new text. 
                //Leave words in upper case unchanged, unless they appear in the exception list.
                foreach (string word in words)
                {
                    counter++;
                    nextWord = counter < words.Count ? words.ElementAt(counter) : nextWords != null && nextWords.Any() ? nextWords.First() : null;


                    if (Regex.IsMatch(word, matchInterpunctuation) || string.IsNullOrWhiteSpace(word))
                    {
                        //punctuation
                        text = text + word;
                    }
                    else if (counter == 1 && i == 0) // overall first word, i.e. first word in first textunit
                    {
                        text = text + ToUpperFirstLetter(word, fullUpperCaseTreatment, culture);
                    }
                    else if (word.Length == 1 && !string.IsNullOrEmpty(nextWord) && nextWord == ".")
                    {
                        //one letter word followed by period is considered a first name initial
                        text = text + ToUpperFirstLetter(word, fullUpperCaseTreatment, culture);
                    }
                    else if (
                        (Regex.IsMatch(prevWord, matchInterpunctuation)) ||
                        (!string.IsNullOrWhiteSpace(secondPrevWord) && Regex.IsMatch(secondPrevWord, matchInterpunctuation) && string.IsNullOrWhiteSpace(prevWord))
                    )
                    {
                        text = text + ToUpperFirstLetter(word, fullUpperCaseTreatment, culture); //capitalize also stopwords directly after interpunctuation
                    }
                    else if (Regex.IsMatch(prevWord, matchApostrophe) && !string.IsNullOrWhiteSpace(secondPrevWord))
                    {
                        text = text + word.ToLower(culture);
                    }
                    else if (Regex.IsMatch(prevWord, matchQuotationMarks)) // capitalize also stopwords directly after quotation marks
                    {
                        text = text + ToUpperFirstLetter(word, fullUpperCaseTreatment, culture);
                    }
                    else if (exceptions.Contains(word.ToLower(culture))) // check list of exceptions
                    {
                        text = text + word.ToLower(culture);
                    }
                    else // in all other cases: capitalize
                    {
                        text = text + ToUpperFirstLetter(word, fullUpperCaseTreatment, culture);
                    }
                    secondPrevWord = prevWord;
                    prevWord = word; // save current word as previous word for next iteration
                }
                textUnits[i].Text = text;
            }

            handled = true;
            return textUnits;
        }

        public string ToUpperFirstLetter(string input, bool ensureAllButFirstIsLower = false, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(input)) return input;

            char[] letters = input.ToCharArray();

            for (var i = 0; i < letters.Length; i++)
            {
                if (i == 0)
                {
                    letters[0] = char.ToUpper(letters[0], culture);
                    continue;
                }

                if (i > 0 && ensureAllButFirstIsLower == false) break;
                letters[i] = char.ToLower(letters[i], culture);
            }

            return new string(letters);
        }

        public enum ConvertFullUpperCaseWords
        {
            Never,
            Always,
            Auto        //converts full uppercase words to lower case only if the conmplete text is written in uppercase
        };

        public bool HasLowerCase(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Any(c => char.IsLower(c));
        }
    }
}
