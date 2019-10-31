//C6#BSO005
//C5
//31.10.2019
//Description: Sortiert Gerichtsentscheidungen im Literaturverzeichnis oder in Mehrfachnachweisen nach Instanz (von der höchsten bis nur niedrigsten)
//Description: Sorts court decisions in the bibliography or in multiple citations by court instance, from hightest to lowest
//Version 1.1: Added ability to configure the field that is to be used for ranking court instances (line 43, "courtField")
//Das Feld zur Bestimmung der Gerichtsinstanz kann in Zeile 43 konfiguriert werden.

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
			// Dieses Script kann sowohl zur Sortierung des Literaturverzeichnisses 
			// als auch von Mehrfachnachweisen im Text und in der Fußnote eingesetzt werden.
			
			//1. Alles ausser Gesetz/Verordnung und Gerichtsentscheidungen
			//2. Gesetz/Verordnung
			//3. Gerichtsentscheidungen
			
			//wenn beide unter 3. fallen:
			//Freitextfeld 1 auswerten ("Instanz", intern CustomField1) ODER Gericht (intern "Organizations"), s. Zeile 34, "courtField"
			//1. Bundesverfassungsgericht
			//2. Bundesgerichtshof
			//3. Oberlandesgericht
			//4. Landgericht
			//5. Amtsgericht
			
			//(die genauen Schreibweisen können in den Zeilen 120-133 eingestellt werden)
			//wenn beide dieselbe Instanz haben absteigend nach Autor, Herausgeber, Institution !
			
			ReferencePropertyId courtField = ReferencePropertyId.Organizations; //alternativ: ReferencePropertyId.CustomField1

			if (x == null || y == null) return 0;
			var xReference = x.Reference;
			var yReference = y.Reference;
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
				
				int xCourtRanking = 99;
				int yCourtRanking = 99;
				
				if (courtField == ReferencePropertyId.Organizations)
				{
					IEnumerable<Person> xCourts = xReference.GetValue(courtField) as IEnumerable<Person>;
					IEnumerable<Person> yCourts = yReference.GetValue(courtField) as IEnumerable<Person>;
					
					Person xCourt = xCourts == null ? null : xCourts.FirstOrDefault();
					Person yCourt = yCourts == null ? null : yCourts.FirstOrDefault();
					
					xCourtRanking = GetCourtRanking(xCourt);
					yCourtRanking = GetCourtRanking(yCourt);
				}
				else
				{
					string xCourt = xReference.GetValue(courtField) as string;
					string yCourt = yReference.GetValue(courtField) as string;
					
					xCourtRanking = GetCourtRanking(xCourt);
					yCourtRanking = GetCourtRanking(yCourt);
				}

				
				
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
			
			if (court.Contains("Bundesverfassungsgericht")
				|| court.Contains("BVerfG")) return 1;
			
			if (court.Contains("Bundesgerichtshof")
				|| court.Contains("BGH")) return 2;
			
			if (court.Contains("Oberlandesgericht")
				|| court.Contains("OLG")) return 3;
			
			if (court.Contains("Landgericht")
				|| court.Contains("LG")) return 4;
			
			if (court.Contains("Amtsgericht")
				|| court.Contains("AG")) return 5;
			
			return 99;
		}
		
		private int GetCourtRanking(Person court)
		{
			if (court == null) return GetCourtRanking(null as string);
			
			int ranking = GetCourtRanking(court.LastName);
			if (ranking != 99) return ranking;
			
			return GetCourtRanking(court.Abbreviation);
		}
	}
}
