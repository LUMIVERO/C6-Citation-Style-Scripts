//CDA002 v3.0
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Drawing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SwissAcademic.Citavi.Citations
{
    public class ComponentPartFilter
		:
		IComponentPartFilter
	{
		//Version 3.0: complete overhaul, script considers different output for placehoder citations and bibliography citations
		//Version 2.0: script can be attached to both date/time field element as well as text field element

		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{
			//enter the culture the date info has been formatted and entered in, e.g. 12/05/2017 would be December, 12th in en-UK and May, 5th in en-US
			CultureInfo targetCulture = CultureInfo.CreateSpecificCulture("en-US");
			//list all possible date formats for this script to check; the scripts tries to parse the date beginning from left to right
			string[] formats = new string[] { "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy", "yyyy/dd/MM",  "dd.MM.yyyy", "d.M.yyyy", "d.MM.yyyy", "dd.M.yyyy", "dd.MM.yy", "d.M.yy", "d.MM.yy", "dd.M.yy" };

			bool usePeriodAfterAbbreviatedMonthName = true;    //if true, month names will be: Jan. Feb. Mar. Apr. May (!) Jun. Jul. Aug. Sept. Oct. Nov. Dec.

			///IMPORTANT: Use the following indexed placeholders {n} and format strings xxxx as in {n:xxxx} for the templates. 
            ///You can ommit placeholders and/or place them freely inside the templates below. Yet, it is not recommended to use the same placeholder more than once, 
            ///because this script is not optimized for this.
            ///
			///{0}: letter for ambiguity resolving
			///{1}: year	of start or single date
			///{2}: month	of start or single date
			///{3}: day		of start or single date
			///{4}: year	of end date
			///{5}: month	of end date
			///{6}: day		of end date
			///use the following formatting for "6 June 2018"
			///YEAR:	yyyy = 2018, yy = 18
			///MONTH:	MMMM = June, MMM = Jun, MM = 06, %M = 6
			///DAY:		dd = 06, %d = 6, %do = 6th

			//SINGLE DATE - output format templates
			string outputFormatSingleDatePlaceholder = "{1:yyyy}{0}";																					//e.g. 2013a
			string outputFormatSingleDateBibliography = "{1:yyyy}{0}, {2:MMMM} {3:%do}";																//e.g. 2013a, January 6th

			//DATE RANGE - output format templates
			//same year, same month
			string outputFormatDateRangeSameYearSameMonthPlaceholder = "{1:yyyy}{0}";																	//e.g. 2013a
			string outputFormatDateRangeSameYearSameMonthBibliography = "{1:yyyy}{0}, {2:MMMM} {3:%do} - {6:%do}";										//e.g. 2013a, January 6th

			//same year, different month
			string outputFormatDateRangeSameYearDifferentMonthPlaceholder = "{1:yyyy}{0}";																//e.g. 2013a
			string outputFormatDateRangeSameYearDifferentMonthBibliography = "{1:yyyy}{0}, {2:MMMM} {3:%do} - {5:MMMM} {6:%do}";						//e.g. 2013a, September 28th - October 3rd

			//different years
			string outputFormatDateRangeDifferentYearsPlaceholder = "{1:yyyy}/{4:yyyy}{0}";																//e.g. 2013/2014a
			string outputFormatDateRangeDifferentYearsBibliography = "{1:yyyy}/{4:yyyy}{0}; {1:yyyy}, {2:MMMM} {3:%do} - {4:yyyy}, {5:MMMM} {6:%do}";	//e.g. 2013/2014a; 2013, December 29th - 2014, January 4th

			handled = false;

			if (citation == null) return null;

			Reference referenceInScope = GetReferenceInScope(componentPart, citation);
			if (referenceInScope == null) return null;

			FieldElement dateFieldElement = GetDateFieldElement(componentPart);
			if (dateFieldElement == null) return null;
			
			ReferencePropertyId referencePropertyId = dateFieldElement.PropertyId;
			string dateString = referenceInScope.GetValue(referencePropertyId) as string;
			if (string.IsNullOrEmpty(dateString)) return null;

			TextUnitCollection output = null;

			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			bool isPlaceholderCitation = placeholderCitation != null;

			PreviewCitation previewCitation = citation as PreviewCitation;
			bool isPreviewBibliographyCitation = previewCitation != null && citation.CitationType == CitationType.Bibliography;

			BibliographyCitation bibliographyCitation = citation as BibliographyCitation;
			bool isBibliographyCitation = bibliographyCitation != null;
			if (bibliographyCitation == null && placeholderCitation != null)
			{
				bibliographyCitation = placeholderCitation.CorrespondingBibliographyCitation;
			}
			if (bibliographyCitation == null && !isPreviewBibliographyCitation) return null;


			string identifyingLetter = bibliographyCitation != null ? bibliographyCitation.IdentifyingLetter : string.Empty;
			LiteralTextUnit identifyingLetterTextUnit = new LiteralTextUnit(identifyingLetter, Drawing.FontStyle.Neutral);
			bool hasIdentifyingLetter = !string.IsNullOrEmpty(identifyingLetter);

			#region Tread n.d. + letter for disambiguation ("IdentifyingLetter")

			if (hasIdentifyingLetter && ContainsND(dateString))
			{
				//we make sure the IdentifyingLetter is separated from n.d. by a space char or hyphen: Smith n.d.-a, Smith n.d.-b
				//go to method SeparateIdentifyingLetterFromND below to customize
				output = componentPart.GetTextUnitsUnfiltered(citation, template);
				if (output == null || !output.Any()) return null;

				handled = true;
				return SeparateIdentifyingLetterFromND(output, identifyingLetter);
			}

			#endregion

			FontStyle fontStyle = dateFieldElement is DateTimeFieldElement ? ((DateTimeFieldElement)dateFieldElement).FontStyle : ((TextFieldElement)dateFieldElement).FontStyle;

			DateTime dateSingle;
			DateTime dateStart;
			DateTime dateEnd;

			string outputText = string.Empty;

            #region Check for Single Date

            if (TryParseSingleDate(dateString, formats, targetCulture, out dateSingle))
			{
                #region BibliographyCitation

                if (isBibliographyCitation || isPreviewBibliographyCitation)
				{
					outputText = FormatDate(dateSingle, outputFormatSingleDateBibliography, targetCulture, identifyingLetter, usePeriodAfterAbbreviatedMonthName); 
				}

                #endregion

                #region PlaceholderCitation

                else if (isPlaceholderCitation)
                {
					outputText = FormatDate(dateSingle, outputFormatSingleDatePlaceholder, targetCulture, identifyingLetter, usePeriodAfterAbbreviatedMonthName);
				}

				#endregion

				#region Other

				else
				{
					handled = false;
					return null;
                }

                #endregion 
			}

            #endregion

            #region Check for Date Range

            else if (TryParseDateRange(dateString, formats, targetCulture, out dateStart, out dateEnd))
			{
                #region BibliographyCitation

                if (isBibliographyCitation || isPreviewBibliographyCitation)
                {
					
					#region same year, same month
                    
					if (dateStart.Year == dateEnd.Year && dateStart.Month == dateEnd.Month && dateStart.Day != dateEnd.Day)
					{
						outputText = FormatDateRange(dateStart, dateEnd, outputFormatDateRangeSameYearSameMonthBibliography, targetCulture, identifyingLetter, usePeriodAfterAbbreviatedMonthName);
					}

                    #endregion

					#region same year, different months
                    
					else if (dateStart.Year == dateEnd.Year && dateStart.Month != dateEnd.Month)
					{
						outputText = FormatDateRange(dateStart, dateEnd, outputFormatDateRangeSameYearDifferentMonthBibliography, targetCulture, identifyingLetter, usePeriodAfterAbbreviatedMonthName);
					}

                    #endregion

                    #region different years

                    else
                    {
						outputText = FormatDateRange(dateStart, dateEnd, outputFormatDateRangeDifferentYearsBibliography, targetCulture, identifyingLetter, usePeriodAfterAbbreviatedMonthName);
					}

					#endregion
				}

                #endregion

                #region PlaceholderCitation

                else if (isPlaceholderCitation)
                {
					#region same year, same month
					
					if (dateStart.Year == dateEnd.Year && dateStart.Month == dateEnd.Month && dateStart.Day != dateEnd.Day)
					{
						outputText = FormatDateRange(dateStart, dateEnd, outputFormatDateRangeSameYearSameMonthPlaceholder, targetCulture, identifyingLetter, usePeriodAfterAbbreviatedMonthName);
					}

					#endregion

					#region same year, different months

					else if (dateStart.Year == dateEnd.Year && dateStart.Month != dateEnd.Month)
					{
						outputText = FormatDateRange(dateStart, dateEnd, outputFormatDateRangeSameYearDifferentMonthPlaceholder, targetCulture, identifyingLetter, usePeriodAfterAbbreviatedMonthName);
					}

                    #endregion

                    #region different years

                    else
                    {
						outputText = FormatDateRange(dateStart, dateEnd, outputFormatDateRangeDifferentYearsPlaceholder, targetCulture, identifyingLetter, usePeriodAfterAbbreviatedMonthName);
					}

                    #endregion 
                }

                #endregion

                #region Other

                else
                {
					handled = false;
					return null;
				}

				#endregion
			}

            #endregion

            #region Do the output

			if (!string.IsNullOrEmpty(outputText))
            {
				var outputTextUnits = new TextUnitCollection();
				outputTextUnits = TextUnitCollectionUtility.TaggedTextToTextUnits(dateFieldElement, outputText, fontStyle);

				if (outputTextUnits.Any())
				{
					List<ITextUnit> componentPartOutput = new List<ITextUnit>();
					foreach(IElement element in componentPart.Elements)
                    {
						if (element == dateFieldElement)
                        {
							componentPartOutput.AddRange(outputTextUnits);
                        }
						else
                        {
							componentPartOutput.AddRange(element.GetTextUnits(citation, template));
                        }
                    }
					handled = true;
					return componentPartOutput;
				}
			}

            #endregion 

            handled = false;
			return null;
		}

        #region GetReferenceInScope

        private Reference GetReferenceInScope(ComponentPart componentPart, Citation citation)
        {
			if (citation == null || citation.Reference == null) return null;
			if (componentPart == null) return null;

			Reference referenceInScope = citation.Reference;
			if (componentPart.Scope == ComponentPartScope.ParentReference) referenceInScope = citation.Reference.ParentReference;
			return referenceInScope;
		}

        #endregion

        #region GetDateFieldElement

        private FieldElement GetDateFieldElement(ComponentPart componentPart)
        {
			if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;

			FieldElement dateFieldElement = componentPart.Elements.Where(element =>
			{
				DateTimeFieldElement dateTimeFieldElement = element as DateTimeFieldElement;
				if (dateTimeFieldElement != null) return true;

				TextFieldElement textFieldElement = element as TextFieldElement;
				if (textFieldElement != null) return true;

				return false;
			}).FirstOrDefault() as FieldElement;

			return dateFieldElement;
		}

        #endregion

        #region ContainsND

        private bool ContainsND(string dateString)
		{
			if (string.IsNullOrEmpty(dateString)) return false;
			string ndRegexPattern = @"(?<nd>n\.{0,1} *d\.{0,1} *)";
			return Regex.IsMatch(dateString, ndRegexPattern);
		}

        #endregion

        #region SeparateIdentifyingLetterFromND

        private TextUnitCollection SeparateIdentifyingLetterFromND(TextUnitCollection input, string identifyingLetter)
        {
			string ndRegexPattern = @"(?<nd>n\.{0,1} *d\.{0,1} *)(?<identifyingLetter>" + identifyingLetter + @")$";
			string ndMatch = "";
			var index = -1;
			bool found = false;

			foreach (ITextUnit textUnit in input)
			{
				index++;
				var match = Regex.Match(textUnit.Text, ndRegexPattern);
				if (match.Success)
				{
					ndMatch = match.Groups["nd"].Value;
					found = true;
					break;
				}
			}

			if (found)
			{
				input.ElementAt(index).Text = ndMatch;
				LiteralTextUnit identifyingLetterTextUnit = new LiteralTextUnit("–" + identifyingLetter, Drawing.FontStyle.Neutral);
				input.Insert(index + 1, identifyingLetterTextUnit);
			}

			//input.Insert(0, new LiteralTextUnit("(", Drawing.FontStyle.Neutral)); //we put parentheses on component part level again
			//input.Add(new LiteralTextUnit(")", Drawing.FontStyle.Neutral));

			return input;
		}

        #endregion

        #region TryParseSingleDate

        private bool TryParseSingleDate(string dateString, string[] formats, CultureInfo targetCulture, out DateTime dateSingle)
        {
			dateSingle = DateTime.MinValue;

			if (string.IsNullOrEmpty(dateString)) return false;
			if (formats == null || !formats.Any()) return false;
			if (targetCulture == null) return false;

			return DateTime.TryParseExact(dateString, formats, targetCulture, DateTimeStyles.None, out dateSingle);
			
        }

        #endregion

        #region TryParseDateRange

        private bool TryParseDateRange(string dateString, string[] formats, CultureInfo targetCulture, out DateTime dateStart, out DateTime dateEnd)
		{
			dateStart = DateTime.MinValue;
			dateEnd = DateTime.MinValue;

			List<string> dates = dateString.Split('-').Select(d => d.Trim()).ToList();
			if (dates.Count != 2)
			{
				return false;
			}

			var foundA = DateTime.TryParseExact(dates.ElementAt(0), formats, targetCulture, DateTimeStyles.None, out dateStart);
			var foundB = DateTime.TryParseExact(dates.ElementAt(1), formats, targetCulture, DateTimeStyles.None, out dateEnd);

			if (!foundA || !foundB)
			{
				return false;
			}

			return true;
		}

		#endregion

		#region FormatDate

		private string FormatDate(DateTime date, string formatTemplate, CultureInfo culture, string identifyingLetter, bool usePeriodAfterAbbreviatedMonthName)
        {
			var formatPlaceholders = GetFormatPlaceholders(formatTemplate);

			object year = (object)date;   //1
			object month = (object)date;  //2
			object day = (object)date;    //3

			if (formatPlaceholders.Month != null && formatPlaceholders.Month.FormatString != null && formatPlaceholders.Month.FormatString.Equals("MMM"))
			{
				month = (object)GetMonthNameAbbreviated(date, culture, usePeriodAfterAbbreviatedMonthName);
			}
			if (formatPlaceholders.Day != null && formatPlaceholders.Day.FormatString != null && formatPlaceholders.Day.FormatString.Equals("%do"))
			{
				day = (object)GetOrdinalStringShort(date.Day, culture);
			}

			return string.Format(culture, formatTemplate, identifyingLetter, year, month, day);
		}

		#endregion

		#region FormatDateRange

		private string FormatDateRange(DateTime startDate, DateTime endDate, string formatTemplate, CultureInfo culture, string identifyingLetter, bool usePeriodAfterAbbreviatedMonthName)
        {
			var formatPlaceholders = GetFormatPlaceholders(formatTemplate);

			object startYear = (object)startDate;   //1
			object startMonth = (object)startDate;  //2
			object startDay = (object)startDate;    //3
			object endYear = (object)endDate;       //4
			object endMonth = (object)endDate;      //5
			object endDay = (object)endDate;        //6

			if (formatPlaceholders.StartMonth != null && formatPlaceholders.StartMonth.FormatString != null && formatPlaceholders.StartMonth.FormatString.Equals("MMM"))
			{
				startMonth = (object)GetMonthNameAbbreviated(startDate, culture, usePeriodAfterAbbreviatedMonthName);
			}
			if (formatPlaceholders.EndMonth != null && formatPlaceholders.EndMonth.FormatString != null && formatPlaceholders.EndMonth.FormatString.Equals("MMM"))
			{
				endMonth = (object)GetMonthNameAbbreviated(endDate, culture, usePeriodAfterAbbreviatedMonthName);
			}
			if (formatPlaceholders.StartDay != null && formatPlaceholders.StartDay.FormatString != null && formatPlaceholders.StartDay.FormatString.Equals("%do"))
			{
				startDay = (object)GetOrdinalStringShort(startDate.Day, culture);
			}
			if (formatPlaceholders.EndDay != null && formatPlaceholders.EndDay.FormatString != null &&  formatPlaceholders.EndDay.FormatString.Equals("%do"))
			{
				endDay = (object)GetOrdinalStringShort(endDate.Day, culture);
			}

			return string.Format(culture, formatTemplate, identifyingLetter, startYear, startMonth, startDay, endYear, endMonth, endDay);
		}

		#endregion

		#region GetMonthNameAbbreviated

		private string GetMonthNameAbbreviated(DateTime inputDate, CultureInfo targetCulture, bool usePeriodAfterAbbreviatedMonthName)
        {
			string monthNameFull = inputDate.ToString("MMMM", targetCulture);
			string monthNameAbbreviated = inputDate.ToString("MMM", targetCulture);

			if (usePeriodAfterAbbreviatedMonthName)
			{
				//CAUTION: some target cultures already come with a period
				monthNameAbbreviated = monthNameAbbreviated.Trim('.');
				return monthNameAbbreviated == monthNameFull ? monthNameAbbreviated : monthNameAbbreviated + ".";
			}
			else
            {
				return monthNameAbbreviated.TrimEnd('.');
            }
        }

        #endregion

        #region GetDayNameAbbreviated

        private string GetDayNameAbbreviated(DateTime inputDate, CultureInfo targetCulture, bool usePeriodAfterAbbreviatedDayName)
        {
			string dayNameFull = inputDate.ToString("dddd", targetCulture);
			string dayNameAbbreviated = inputDate.ToString("ddd", targetCulture);

			if (usePeriodAfterAbbreviatedDayName)
            {
				return dayNameAbbreviated == dayNameFull ? dayNameAbbreviated : dayNameAbbreviated + ".";
            }
			else
            {
				return dayNameAbbreviated;
            }
        }

		#endregion

		#region GetOrdinalString

		private string GetOrdinalStringShort(int inputNumber, CultureInfo targetCulture)
		{
			//can be improved by font (e.g. <sup>) and gender (e.g. 1re 1er)
			//default for positive integers is currently just adding a "." as in German below

			if (inputNumber <= 0) return inputNumber.ToString();

			string twoLetterISOLanguageName = targetCulture == null ? "en" : targetCulture.TwoLetterISOLanguageName;

			switch (twoLetterISOLanguageName)
			{
				#region English

				case "en":
					{
						switch (inputNumber % 100)
						{
							case 11:
							case 12:
							case 13:
								return inputNumber + "<sup>th</sup>";
						}

						switch (inputNumber % 10)
						{
							case 1:
								return inputNumber + "<sup>st</sup>";
							case 2:
								return inputNumber + "<sup>nd</sup>";
							case 3:
								return inputNumber + "<sup>rd</sup>";
							default:
								return inputNumber + "<sup>th</sup>";
						}
					}
					break;

				#endregion

				#region French

				//1st: male: er, female: re, 2nd-.... e
				case "fr":
					{
						return inputNumber + "e";
					}
					break;

				#endregion

				#region Spanish

				//1st 1ro primero; 2nd 2do segundo; 3rd 3er tercero; 4th 4to cuarto; 5th 5to quinto; 6th 6to sexto; 7th 7mo séptimo; 8th 8vo octavo; 9th 9no noveno; 10th 10mo décimo
				//11th 11mo undécimo; 12th 12mo duodécimo; 13th 13cio decimotercio; 

				case "es":
					{
						return inputNumber + ".";
					}
					break;

				#endregion

				#region German

				//just a dot

				case "de":
				default:
					{
						return inputNumber + ".";
					}

					#endregion
			}
		}

		#endregion

		#region GetFormatPlaceholders

		private FormatPlaceholders GetFormatPlaceholders(string formatTemplate, int max_count = 100)
        {
			//we return only placeholders with index 0, 1, 2, 3, 4, 5, 6
			//if an index is NOT used, we return null
			var formatTemplateParser = new FormatTemplateParser();

			
			string.Format(formatTemplateParser, formatTemplate, Enumerable.Range(0, max_count).Cast<object>().ToArray());
			var placeholders = formatTemplateParser.Placeholders;

			var returnValue = new FormatPlaceholders();

			foreach(var placeholder in placeholders)
            {
				switch (placeholder.Index)
                {
					case 0: 
						returnValue.IdentifyingLetter = placeholder;
						break;
					case 1:
						returnValue.StartYear = placeholder;
						break;
					case 2:
						returnValue.StartMonth = placeholder;
						break;
					case 3:
						returnValue.StartDay = placeholder;
						break;
					case 4:
						returnValue.EndYear = placeholder;
						break;
					case 5:
						returnValue.EndMonth = placeholder;
						break;
					case 6:
						returnValue.EndDay = placeholder;
						break;
                }
            }

			return returnValue;
        }

		#endregion

		#region Class FormatParser

		//https://stackoverflow.com/questions/948303/is-there-a-better-way-to-count-string-format-placeholders-in-a-string-in-c/948876#948876

		private class FormatTemplateParser
			:
			IFormatProvider,
            ICustomFormatter
        {
			internal readonly List<FormatPlaceholder> Placeholders = new List<FormatPlaceholder>();

			public object GetFormat(Type formatType)
            {
				return this;
            }

			public string Format(string formatString, object arg, IFormatProvider formatProvider)
            {
				var index = (int)arg;
				Placeholders.Add(new FormatPlaceholder((int)arg, formatString));
				return null;
			}
        }

        #endregion

        #region Class FormatPlaceholder

		private class FormatPlaceholder
        {
			#region Properties

			public int Index { get; private set; }
			public string FormatString { get; private set; }

			#endregion

			public FormatPlaceholder(int index, string formatString)
            {
				Index = index;
				FormatString = formatString;
            }
        }

		#endregion

		#region Class FormatPlaceholders

		private class FormatPlaceholders
        {
			public FormatPlaceholder IdentifyingLetter { get; set; }

			public FormatPlaceholder Year 
			{ 
				get
                {
					return StartYear;
                }
			}

			public FormatPlaceholder Month 
			{ 
				get
                {
					return StartMonth;
                }
			}

			public FormatPlaceholder Day 
			{ 
				get
                {
					return StartDay;
                }
			}

			public FormatPlaceholder StartYear { get; set; }

			public FormatPlaceholder StartMonth { get; set; }

			public FormatPlaceholder StartDay { get; set; }

			public FormatPlaceholder EndYear { get; set; }

			public FormatPlaceholder EndMonth { get; set; }

			public FormatPlaceholder EndDay { get; set; }
        }

        #endregion 
    }
}
