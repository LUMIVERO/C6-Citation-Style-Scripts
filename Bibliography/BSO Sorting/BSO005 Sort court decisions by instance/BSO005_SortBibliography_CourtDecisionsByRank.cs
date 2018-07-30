//BSO005
//21.06.2018 
//Version 1.0

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
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
		
			//1. Alles ausser Gesetz/Verordnung und Gerichtsentscheidungen
			//2. Gesetz/Verordnung
			//3. Gerichtsentscheidungen
			
			//wenn beide unter 3. fallen:
			//Freitextfeld 1 auswerten ("Instanz");
			//1. Bundesverfassungsgericht
			//2. Bundesgerichtshof
			//3. Oberlandesgericht
			//4. Landgericht
			//5. Amtsgericht
			
			//wenn beide dieselbe Instanz haben absteigend nach Autor, Herausgeber, Institution !

			var xBibliographyCitation = x as BibliographyCitation;
			var yBibliographyCitation = y as BibliographyCitation;

			if (xBibliographyCitation == null || yBibliographyCitation == null) return 0;
			var xReference = xBibliographyCitation.Reference;
			var yReference = yBibliographyCitation.Reference;
			if (xReference == null || yReference == null) return 0;		
			
			var defaultComparer = CitationComparer.AuthorYearTitleOrNoAuthorThenTitleYearAscending;
		
			
			var xIsLiterature = xReference.ReferenceType != ReferenceType.StatuteOrRegulation && xReference.ReferenceType != ReferenceType.CourtDecision;
			var yIsLiterature = yReference.ReferenceType != ReferenceType.StatuteOrRegulation && yReference.ReferenceType != ReferenceType.CourtDecision;
			
			var xIsStatuteOrRegulation = xReference.ReferenceType == ReferenceType.StatuteOrRegulation;
			var yIsStatuteOrRegulation = yReference.ReferenceType == ReferenceType.StatuteOrRegulation;
			
			var xIsCourtDecision = xReference.ReferenceType == ReferenceType.CourtDecision;
			var yIsCourtDecision = yReference.ReferenceType == ReferenceType.CourtDecision;
			
			if (xIsCourtDecision && yIsCourtDecision)
			{
				//spezielle Sortierung für Gerichtsentscheidungen:
				
				int xCourtRanking = GetCourtRanking(xReference.CustomField1);
				int yCourtRanking = GetCourtRanking(yReference.CustomField1);
				
				if (xCourtRanking == yCourtRanking)
				{
					return defaultComparer.Compare(x, y);
				}
				else
				{
					return xCourtRanking.CompareTo(yCourtRanking);
				}	
				
			}
			else if (xIsCourtDecision && !yIsCourtDecision)
			{
				return 1;
			}
			else if (!xIsCourtDecision && yIsCourtDecision)
			{
				return -1;
			}
			else if (xIsStatuteOrRegulation && yIsLiterature)
			{
				return 1;
			}
			else if (xIsLiterature && yIsStatuteOrRegulation)
			{
				return -1;
			}
			else
			{
				//z.B. beide Literatur oder beide Gesetz/Verordnung
				return defaultComparer.Compare(x, y);
			}

		}
		
		private int GetCourtRanking(string court)
		{
			if (string.IsNullOrEmpty(court)) return 99;
			
			if (court == "Bundesverfassungsgericht") return 1;
			if (court == "Bundesgerichtshof") return 2;
			if (court == "Oberlandesgericht") return 3;
			if (court == "Landgericht") return 4;
			if (court == "Amtsgericht") return 5;
			
			return 99;
		
		}
	}
}