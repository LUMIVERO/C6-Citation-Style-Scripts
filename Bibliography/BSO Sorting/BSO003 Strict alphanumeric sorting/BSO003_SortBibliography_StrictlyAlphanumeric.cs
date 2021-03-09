//BSO003
//21.06.2018 
//Version 1.1

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Citations;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;

namespace SwissAcademic.Citavi.Comparers
{
	public class CustomCitationComparer
		:
		ICustomCitationComparerMacro
	{
		public int Compare(Citation x, Citation y)
		{

			//First we make sure we are comparing BibliographyCitations only
			var xBibliographyCitation = x as BibliographyCitation;
			var yBibliographyCitation = y as BibliographyCitation;

			if (xBibliographyCitation == null || yBibliographyCitation == null) return 0;
			var xReference = xBibliographyCitation.Reference;
			var yReference = yBibliographyCitation.Reference;
			if (xReference == null || yReference == null) return 0;


			var xBibliographyEntry = GetTextForSorting(x.GetTextUnits().ToString());
			var yBibliographyEntry = GetTextForSorting(y.GetTextUnits().ToString());
			
			return xBibliographyEntry.CompareTo(yBibliographyEntry);
		}
		
		private string GetTextForSorting (string text, bool removeLeadingParticles = false)
		{
			var particlesToIgnore = new string[] {
				"am",
				"an",
				"auf",
				"das",
				"der",
				"die",
				"ein",
				"eine",
				"im",
				"in",
				"inmitten",
				"nach",
				"über",
				"ueber",
				"vom",
				"von",
				"von",
				"was",
				"wie",
				"wo",
				"zu",
				"zum",
				"zur",
				"zwischen",

				"a",
				"the",
				"an",
				
				"le",
				"la",
				"les"
			};
				
			var particlesRegEx = new Regex(@"\b(" + string.Join("|", particlesToIgnore) + @")\b", RegexOptions.IgnoreCase);
			//var wordsRegex = new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*");	//use Split
			var wordsRegex = new Regex(@"\w+[^\s]*\w+|\w");				//use Matches
			
			//string[] words = wordsRegex.Split(text);
			string[] words = wordsRegex.Matches(text).Cast<Match>().Select(m => m.Value).ToArray();
			
			if (removeLeadingParticles)
			{
				int firstNonParticleToIgnoreIndex = -1;
				foreach (string word in words)
				{
					if (string.IsNullOrEmpty(word)) break;

					firstNonParticleToIgnoreIndex++;
					if (!particlesRegEx.IsMatch(word)) break;
				}
				
				if (firstNonParticleToIgnoreIndex == -1)
				{
					return string.Join(" ", words);
				}
				else
				{
					return string.Join(" ", words, firstNonParticleToIgnoreIndex, words.Length - firstNonParticleToIgnoreIndex);
				}
			}
			else
			{
				return string.Join(" ", words);
			}
		}
	}
}
