//C6#CPE003-A - SpaceOnly
//C5#43123
//Description:	Changes Output Style of Journal Abbreviation (using "Abbreviation 1" and "Abbreviation 2" as source)
//Version 3.0:	var A - added extended fallback capabilities
//Version 2.0:	"Abbreviation 2" added as a possible source
//Notes: [1] Period/space (Intl. J. History) DEFAULT - [2] Space only (Intl J History) - [3] Period only (Intl.J.Histry) - [4] No period, no space (IntlJHistory)

using System;
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
			//Change this to .PeriodSpace, .PeriodOnly, .SpaceOnly or .NoPeriodNoSpace if required
			//[1] Period/space 			(Intl. J. History) - DEFAULT
			//[2] Space only 			(Intl J History)
			//[3] Period only 			(Intl.J.Histry)
			//[4] No period, no space 	(IntlJHistory)
			PeriodicalAbbreviationOutputStyle outputStyle = PeriodicalAbbreviationOutputStyle.SpaceOnly;
			
			//if true and if the option "Fall back to next available name" was ticked on the field element, the script first tries the built in fallback process, starting from the abbreviation set on the field element
			//UserAbbreviation2 >>> UserAbbreviation1 >>> StandardAbbreviation
			//if that does not yield a non-empty abbreviation, the other direction is used
			//StandardAbbreviation >>> UserAbbreviation1 >>> UserAbbreviation2
			bool extendedFallback = true;
			
			handled = false;
		
			if (citation == null) return null;
			
			Reference reference = citation.Reference;
			if (reference == null) return null;
			
			if (!citation.Reference.HasCoreField(ReferenceTypeCoreFieldId.Periodical)) return null;
			Periodical periodical = reference.Periodical;
			if (periodical == null) return null;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;
			
			var firstPeriodicalFieldElement =  componentPart.Elements.FirstOrDefault<IElement>(element => element is PeriodicalFieldElement) as PeriodicalFieldElement;
			if (firstPeriodicalFieldElement == null) return null;
			
			//if the PeriodicalFieldElement is set to ignore abbreviations, we exit here
			PeriodicalNameUsage nameUsage = firstPeriodicalFieldElement.PeriodicalNameUsage;
			if (nameUsage == PeriodicalNameUsage.Name) return null;
			
			string abbreviation = String.Empty;
			string periodicalName = periodical.Name;

			#region Determine Abbreviation (Standard Fallback)
			
			bool useStandardFallback = firstPeriodicalFieldElement.Fallback;
		  	if (nameUsage == PeriodicalNameUsage.UserAbbreviation2)
			{
				if (!string.IsNullOrEmpty(periodical.UserAbbreviation2)) 
				{
					//Wenn das gewählte Feld bei einer Zeitschrift gefüllt ist, muss das Skript nicht tätig werden, sondern der Eintrag wird einfach ausgegeben.
					return null;
				}
				
				if (useStandardFallback) abbreviation = periodical.UserAbbreviation1;
				if (string.IsNullOrEmpty(abbreviation) && useStandardFallback) abbreviation = periodical.StandardAbbreviation;
			}
			else if (nameUsage == PeriodicalNameUsage.UserAbbreviation1)
			{
				if (!string.IsNullOrEmpty(periodical.UserAbbreviation1))
				{
					//Wenn das gewählte Feld bei einer Zeitschrift gefüllt ist, muss das Skript nicht tätig werden, sondern der Eintrag wird einfach ausgegeben.
					return null; 
				}
				if (useStandardFallback) abbreviation = periodical.StandardAbbreviation;
			}
			else if (nameUsage == PeriodicalNameUsage.StandardAbbreviation)
			{
				if (!string.IsNullOrEmpty(periodical.StandardAbbreviation))
				{
					return null;
				}
			}
			
			#endregion
			
			#region Determine Abbreviation (Extended Fallback)
			
			bool useExtendedFallback = useStandardFallback && extendedFallback;
			if (string.IsNullOrEmpty(abbreviation) && useExtendedFallback)
			{
				if (nameUsage == PeriodicalNameUsage.StandardAbbreviation)
				{
					abbreviation = periodical.StandardAbbreviation;
					if (string.IsNullOrEmpty(abbreviation)) abbreviation = periodical.UserAbbreviation1;
					if (string.IsNullOrEmpty(abbreviation)) abbreviation = periodical.UserAbbreviation2;
				}
				else if (nameUsage == PeriodicalNameUsage.UserAbbreviation1)
				{
					abbreviation = periodical.UserAbbreviation1;
					if (string.IsNullOrEmpty(abbreviation)) abbreviation = periodical.UserAbbreviation2;
				}
				else if (nameUsage == PeriodicalNameUsage.UserAbbreviation2)
				{
					abbreviation = periodical.UserAbbreviation2;
				}
			}
			
			#endregion
			
			if (string.IsNullOrEmpty(abbreviation)) return null; //no abbreviation found
			
			string[] periodicalNameWords = periodicalName.ToLowerInvariant().Split(new char[]{' ', '.', ';', ',', ':', '&', '-'}, StringSplitOptions.RemoveEmptyEntries);
			string[] abbreviationWords = abbreviation.Split(new string[] { " ", "  ", "   " }, StringSplitOptions.RemoveEmptyEntries);
			//string[] abbreviationWords = abbreviation.Split(' ');
			
			if (!abbreviation.Contains("."))
			{
				List<string> abbreviationWithFullStops = new List<string>();
				  
				foreach (string word in abbreviationWords)
				{
					if (word.StartsWith("(") || word.EndsWith(")"))
					{
						abbreviationWithFullStops.Add(word);
					}
					else if (!Array.Exists(periodicalNameWords, x => x == word.ToLowerInvariant()))
					{
						abbreviationWithFullStops.Add(word + ".");
					}
					else
					{
						abbreviationWithFullStops.Add(word);
					}
				}	  
				abbreviationWords = abbreviationWithFullStops.ToArray();
			}				

			string[] wordsWithoutPeriod = new String[abbreviationWords.Length];
			abbreviationWords.CopyTo(wordsWithoutPeriod, 0);
			for (int i = 0; i < wordsWithoutPeriod.Length; i++)
			{
				wordsWithoutPeriod[i] = wordsWithoutPeriod[i].TrimEnd('.');
			}
			
			var outputTextElement = new LiteralElement(componentPart, abbreviation);
			outputTextElement.FontStyle = firstPeriodicalFieldElement.FontStyle;
			
			switch (outputStyle)
			{
				case PeriodicalAbbreviationOutputStyle.PeriodOnly:
					outputTextElement.Text = string.Join("", abbreviationWords);
					break;

				case PeriodicalAbbreviationOutputStyle.PeriodSpace:
					 outputTextElement.Text = string.Join(" ", abbreviationWords);
					 break;
					
				case PeriodicalAbbreviationOutputStyle.SpaceOnly:
					outputTextElement.Text = string.Join(" ", wordsWithoutPeriod);
					break;
					
				case PeriodicalAbbreviationOutputStyle.NoPeriodNoSpace:
					outputTextElement.Text = string.Join("", wordsWithoutPeriod);
					break;			 
			}
			
			componentPart.Elements.ReplaceItem(firstPeriodicalFieldElement, outputTextElement);
			return null;
		}
		
		private enum PeriodicalAbbreviationOutputStyle
		{
			PeriodSpace,
			SpaceOnly,
			PeriodOnly,
			NoPeriodNoSpace
		}
	}
}
