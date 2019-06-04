//Version 3
//Advanced date formatting
//- specify a culture different from your operating system (e.g. use de-DE or fr-FR on a system using en-US)
//- specify the exact formatting pattern for full dates (2018-08-17), year-month (e.g. 2018-08 or August 2018) and year only (e.g. 2018 or 18)
//- avoid automatic addition of 1st day and 1month for date inputs that don't contain day or month information 

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
			if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			
			#region ReferenceInScope
			
			Reference referenceInScope = null;
			if (componentPart.Scope == ComponentPartScope.Reference)
			{
				referenceInScope = citation.Reference;
			}
			else if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				referenceInScope = citation.Reference.ParentReference;
			}
			if (referenceInScope == null) return null;
			
			#endregion
			
			IEnumerable<DateTimeFieldElement> dateTimeFieldElements = componentPart.Elements.OfType<DateTimeFieldElement>().ToList();
			if (dateTimeFieldElements == null || !dateTimeFieldElements.Any()) return null;
		
		
			bool dateFound = false;
			CultureInfo targetCulture = CultureInfo.CreateSpecificCulture("en-US");
			string targetFullFormat = ""; //use "" and it will be taken from the DateTimeFieldElement
			string targetYearMonthFormat = "yyyy MMM";
			string targetYearOnlyFormat = "yyyy";
			
			foreach (DateTimeFieldElement dateTimeFieldElement in dateTimeFieldElements)
			{
				if (string.IsNullOrEmpty(targetFullFormat)) targetFullFormat = dateTimeFieldElement.Format;
				ReferencePropertyId propertyId = dateTimeFieldElement.PropertyId;
				string dateString = referenceInScope.GetValue(propertyId) as string;
				if (string.IsNullOrEmpty(dateString)) continue;
				
				List<Segment> segments = GetSegments(dateString);
				if (segments == null || !segments.Any()) continue;
				
				List<LiteralElement> literalElements = new List<LiteralElement>();
				foreach (Segment segment in segments)
				{
					switch (segment.type)
					{
						#region Text
						
						case SegmentType.Text:
							{
								var literalElement = new LiteralElement(componentPart, segment.text);
								literalElement.FontStyle = dateTimeFieldElement.FontStyle;
								literalElements.Add(literalElement);
							}
							break;

						#endregion

						#region DateTime
							
						case SegmentType.DateTime:
							{
								string newDateString;
								if (!segment.ContainsMonthInformation && !segment.ContainsDayInformation)
								{
									newDateString = segment.dateTime.ToString(targetYearOnlyFormat, targetCulture);
								}
								else if (!segment.ContainsDayInformation)
								{
									newDateString = segment.dateTime.ToString(targetYearMonthFormat, targetCulture);
								}
								else
								{
									newDateString = segment.dateTime.ToString(targetFullFormat, targetCulture);
								}
								var literalElement = new LiteralElement(componentPart, newDateString);
								literalElement.FontStyle = dateTimeFieldElement.FontStyle;
								literalElements.Add(literalElement);
							}
							break;
							
						#endregion
					}
				}
				
				//replace the DateTimeFieldElement by the LiteralElements
				componentPart.Elements.ReplaceItem(dateTimeFieldElement, literalElements);
			}
		

			
			//and Citavi handles the rest, therefore we say handled = false && return null
			handled = false;
			return null;
		}
		
		#region Segment-ization
		
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
		
		#endregion
	}
}