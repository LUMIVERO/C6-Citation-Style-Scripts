//CDA005

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
			//V1.1  If field has year information only, without month and day, only the year will be output here (instead of January, 1st of that year)


			//CultureInfo targetCulture = new CultureInfo("fr");								//"neutrale" Kultur
			CultureInfo targetCulture = CultureInfo.CreateSpecificCulture("en-US");				//spezifische Kultur



			handled = false;

			if (citation == null) return null;
			if (citation.Reference == null) return null;
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;

			IEnumerable<DateTimeFieldElement> dateTimeFieldElements = componentPart.Elements.OfType<DateTimeFieldElement>();
			if (dateTimeFieldElements == null || dateTimeFieldElements.Count() != 1) return null;

			DateTimeFieldElement dateTimeFieldElement = dateTimeFieldElements.ElementAt(0);

			if (dateTimeFieldElement == null) return null;

			var propertyId = dateTimeFieldElement.PropertyId;
			var dateTimeStringValue = citation.Reference.GetValue(propertyId) as string;
			if (string.IsNullOrEmpty(dateTimeStringValue)) return null;


			//das folgende geht nicht, da DateTimeFieldFormatter leider "internal" ist
			//TextUnitCollection textUnits = DateTimeFieldFormatter.Format(citation, dateTimeFieldElement, dateTimeStringValue);

			List<Segment> segments = GetSegments(dateTimeStringValue);


			List<LiteralElement> literalElements = new List<LiteralElement>();

			TextUnitCollection debug = new TextUnitCollection();
			int counter = 1;
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
							if (!segment.ContainsMonthInformation && !segment.ContainsDayInformation)
							{
								newDateString = segment.dateTime.ToString("yyyy", targetCulture);
							}
							else
							{
								newDateString = segment.dateTime.ToString(dateTimeFieldElement.Format, targetCulture);
							}
							var literalElement = new LiteralElement(componentPart, newDateString);
							literalElement.FontStyle = dateTimeFieldElement.FontStyle;
							literalElements.Add(literalElement);
						}
						break;

				}
			}


			//replace the DateTimeFieldElement by the LiteralElements
			componentPart.Elements.ReplaceItem(dateTimeFieldElement, literalElements);


			//and Citavi handles the rest, therefore we say handled = false && return null
			handled = false;
			return null;
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