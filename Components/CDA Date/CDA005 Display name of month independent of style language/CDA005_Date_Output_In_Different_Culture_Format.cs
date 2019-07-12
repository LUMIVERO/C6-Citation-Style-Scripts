//CDA005
//Description: Output date field (including name of month) in different culture format depending on language of reference
//Version: 1.3 If reference is ambiguous, a letter for disambiguation is added to the output
//Version: 1.2 Lines 107ff language specific output
//Version: 1.1 If field has year information only, without month and day, only the year will be output here (instead of January, 1st of that year)


using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
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
			handled = false;

			if (citation == null) return null;
			if (citation.Reference == null) return null;
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;
			
			bool ambiguityFound = false;
			BibliographyCitation bibliographyCitation = citation as BibliographyCitation;
			if (bibliographyCitation == null)
			{
				PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
				if (placeholderCitation != null)
				{
					bibliographyCitation = placeholderCitation.CorrespondingBibliographyCitation;
				}
			}
			if (bibliographyCitation != null && 
				bibliographyCitation.AmbiguityFound && 
				!string.IsNullOrEmpty(bibliographyCitation.IdentifyingLetter)
			) ambiguityFound = true;
			
			#region Find field elements of type DateTime

			IEnumerable<DateTimeFieldElement> dateTimeFieldElements = componentPart.Elements.OfType<DateTimeFieldElement>();
			if (dateTimeFieldElements == null || dateTimeFieldElements.Count() != 1) return null;

			DateTimeFieldElement dateTimeFieldElement = dateTimeFieldElements.ElementAt(0);
			if (dateTimeFieldElement == null) return null;
			
			#endregion
			
			#region Determine reference to look at
			
			Reference reference;
			if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				if (citation.Reference.ParentReference == null) return null;
				reference = citation.Reference.ParentReference as Reference;
			}
			else
			{
				reference = citation.Reference as Reference;
			}
			if (reference == null) return null;			
			
			#endregion Determine reference to look at
			
			#region Determine reference language
			
			Language language;
			if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("en").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.English;
			}
			else if (String.Equals(reference.LanguageCode, CultureInfo.GetCultureInfo("de").TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
			{
				language = Language.German;
			}
			else
			{
				language = Language.Other;
			}
			
			#endregion Determine reference language

			var propertyId = dateTimeFieldElement.PropertyId;
			var dateTimeStringValue = reference.GetValue(propertyId) as string;
			if (string.IsNullOrEmpty(dateTimeStringValue)) return null;


			//das folgende geht nicht, da DateTimeFieldFormatter leider "internal" ist
			//TextUnitCollection textUnits = DateTimeFieldFormatter.Format(citation, dateTimeFieldElement, dateTimeStringValue);

			List<Segment> segments = GetSegments(dateTimeStringValue);


			List<LiteralElement> literalElements = new List<LiteralElement>();

			TextUnitCollection debug = new TextUnitCollection();
			//int counter = 1;
			foreach (Segment segment in segments)
			{
				switch (segment.type)
				{
					case SegmentType.Text:
						{
							var literalElement = new LiteralElement(componentPart, segment.text);
							literalElement.FontStyle = dateTimeFieldElement.FontStyle;
							literalElements.Add(literalElement);
						}
						break;

					case SegmentType.DateTime:
						{
							string newDateString;
							
							#region YEAR information only
							
							if (!segment.ContainsMonthInformation && !segment.ContainsDayInformation)
							{
								switch (language)
								{
									default:
									case (Language.English):
									{
										newDateString = segment.dateTime.ToString("yyyy", new CultureInfo("en-US")); //or "en-UK"
									}
									break;
									
									case (Language.German):
									{
										newDateString = segment.dateTime.ToString("yyyy", new CultureInfo("de-DE")); // or "de-CH" or "de-AT"
									}
									break;
									
									case (Language.Other):
									{
										newDateString = segment.dateTime.ToString("yyyy", new CultureInfo("en-US"));
									}
									break;	
								}
							}
							
							#endregion
							
							#region YEAR and MONTH
							
							else if (!segment.ContainsDayInformation)
							{
								switch (language)
								{
									default:
									case (Language.English):
									{
										newDateString = segment.dateTime.ToString("MM/yyyy", new CultureInfo("en-US")); //or "en-UK"
									}
									break;
									
									case (Language.German):
									{
										newDateString = segment.dateTime.ToString("MMM yyyy", new CultureInfo("de-DE")); // or "de-CH" or "de-AT"
									}
									break;
									
									case (Language.Other):
									{
										newDateString = segment.dateTime.ToString("MM/yyyy", new CultureInfo("en-US"));
									}
									break;	
								}
							}
							
							#endregion
							
							#region YEAR and MONTH and DAY
							
							else
							{
								switch (language)
								{
									default:
									case (Language.English):
									{
										newDateString = segment.dateTime.ToString("d", new CultureInfo("en-US")); //or "en-UK"
									}
									break;
									
									case (Language.German):
									{
										newDateString = segment.dateTime.ToString("dd. MM yyyy", new CultureInfo("de-DE")); // or "de-CH" or "de-AT"
									}
									break;
									
									case (Language.Other):
									{
										newDateString = segment.dateTime.ToString("MM/dd/yyyy", new CultureInfo("en-US"));
									}
									break;	
								}
							}
							
							#endregion
							
							var literalElement = new LiteralElement(componentPart, newDateString);
							literalElement.FontStyle = dateTimeFieldElement.FontStyle;
							literalElements.Add(literalElement);
						}
						break;

				}
			}

			if (ambiguityFound) literalElements.Add(new LiteralElement(componentPart, bibliographyCitation.IdentifyingLetter));

			//replace the DateTimeFieldElement by the LiteralElements
			componentPart.Elements.ReplaceItem(dateTimeFieldElement, literalElements);


			//and Citavi handles the rest, therefore we say handled = false && return null
			handled = false;
			return null;
		}

		private enum Language
		{
			English,
			German,
			Other
		}

		private struct Segment
		{
			public string text;
			public SegmentType type;
			public DateTime dateTime;
			public bool ContainsDayInformation;
			public bool ContainsMonthInformation;
		}

		private enum SegmentType
		{
			Text,
			DateTime
		}

		private List<Segment> GetSegments(string completeString)
		{
			List<Segment> segments = new List<Segment>();
			if (string.IsNullOrEmpty(completeString)) return segments;

			List<DateTimeMatch> dateTimeMatches = DateTimeInformation.Matches(completeString);

			if (dateTimeMatches.Count == 0)
			{
				segments.Add(new Segment() { text = completeString, type = SegmentType.Text, dateTime = SwissAcademic.Environment.NullDate });
				return segments;
			}

			int currentPosition = 0;
			foreach (DateTimeMatch dateTimeMatch in dateTimeMatches)
			{
				int matchPosition = dateTimeMatch.Match.Index;
				int matchLength = dateTimeMatch.Match.Length;

				if (matchPosition > currentPosition)
				{
					//we add a simple string segment
					segments.Add(new Segment() { text = completeString.Substring(currentPosition, matchPosition - currentPosition), type = SegmentType.Text, dateTime = SwissAcademic.Environment.NullDate });
					currentPosition = matchPosition + matchLength;
				}

				//we add the found date string segment
				segments.Add(new Segment() {
					text = completeString.Substring(matchPosition, matchLength),
					type = SegmentType.DateTime,
					dateTime = dateTimeMatch.DateTime,
					ContainsDayInformation = !dateTimeMatch.MissingDayWasAutoCompleted,
					ContainsMonthInformation = !dateTimeMatch.MissingMonthWasAutoCompleted
				});
				currentPosition = matchPosition + matchLength;
			}

			if (currentPosition <= completeString.Length - 1)
			{
				//we add a remaining string segment
				segments.Add(new Segment() { text = completeString.Substring(currentPosition, completeString.Length - currentPosition), type = SegmentType.Text, dateTime = SwissAcademic.Environment.NullDate });
			}

			return segments;
		}
	}
}
