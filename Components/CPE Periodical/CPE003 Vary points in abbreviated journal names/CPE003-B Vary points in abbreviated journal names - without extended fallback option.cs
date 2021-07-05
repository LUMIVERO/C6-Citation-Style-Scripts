//C6#CPE003-B - Vary points in abbreviated Journal Names - without extended fallback option - SpaceOnly
//C5#43123
//Description:	Changes Output Style of Journal Abbreviation (using "Abbreviation 1" and "Abbreviation 2" as source)
//Version 3.0:	var B - the script now takes into account which abbreviation option was set in the component
//Version 2.0:	"Abbreviation 2" added as a possible source
//Notes: 		[1] Period/space (Intl. J. History) DEFAULT - [2] Space only (Intl J History) - [3] Period only (Intl.J.Histry) - [4] No period, no space (IntlJHistory)

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

			PeriodicalNameUsage nameUsage = firstPeriodicalFieldElement.PeriodicalNameUsage;
			if (nameUsage == PeriodicalNameUsage.Name) return null;								//if the PeriodicalFieldElement is set to ignore abbreviations, we exit here
			if (nameUsage == PeriodicalNameUsage.UserAbbreviation2) return null;				//ditto für UserAbbreviation2, Abkürzung 3
			
			string abbreviation = String.Empty;
			string periodicalName = periodical.Name;

			#region Determine Abbreviation
			
			if (nameUsage == PeriodicalNameUsage.StandardAbbreviation)
			{
				if (!string.IsNullOrEmpty(periodical.StandardAbbreviation))
				{
					//Wenn das gewählte Feld [Abkürzung 1 bzw. 2] bei einer Zeitschrift gefüllt ist, muss das Skript nicht tätig werden, 
					//sondern der Eintrag wird einfach ausgegeben.
					return null;
				}
				else if (!string.IsNullOrEmpty(periodical.UserAbbreviation1))
				{
					//Wenn das gewählte Feld [Abkürzung 1 bzw. 2] bei einer Zeitschrift leer ist, das gegenläufige Abkürzungsfeld (1 vs. 2) aber gefüllt, 
					//dann soll das Skript das entsprechende Format generieren.
					abbreviation = periodical.UserAbbreviation1;
				}
			}
			else if (nameUsage == PeriodicalNameUsage.UserAbbreviation1)
			{
				if (!string.IsNullOrEmpty(periodical.UserAbbreviation1))
				{
					//Wenn das gewählte Feld [Abkürzung 1 bzw. 2] bei einer Zeitschrift gefüllt ist, muss das Skript nicht tätig werden, 
					//sondern der Eintrag wird einfach ausgegeben.
					return null;
				}
				else if (!string.IsNullOrEmpty(periodical.StandardAbbreviation))
				{
					abbreviation = periodical.StandardAbbreviation;
				}
			}
			
			#endregion
			
			//Wenn beide Felder Abkürzung 1 und 2 leer sind, kann und darf das Skript nicht tätig werden. 
			//Wenn dann die Checkbox "Automatisch zurückfallen auf den nächsten verfügbaren Namen" aktiviert ist, wird der vollständige Zeitschriftenname ausgegeben. 
			//Wenn nicht, hängt das Verhalten ja davon ab, ob bei der Komponente das Häkchen vor "Dieses Feld darf nicht leer sein" gesetzt ist, 
			//dementsprechend erscheint ggf. ein Hinweistext oder eben auch nicht.
			if (string.IsNullOrEmpty(abbreviation)) return null;
			
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
