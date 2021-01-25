//C6#COT039
//Description: Copy parallel publication into the Title supplement field
//Version: 1.1: Slight improvements
//
//More precisely, the script outputs the parallel publication as a formatted string and inserts it into the "Title supplement" field.
//Funktion des Komponentenskripts: Gibt die Parallelveröffentlichung formatiert aus und schreibt diese in das Feld "Titelzusätze".
//
//Schritt 1:
//Fügen Sie im Zitationsstil-Editor dieses Skript bei der Komponente "Titelzusätze" ein, wie unter www.citavi.com/programmable_components gezeigt.
//Bitte beachten Sie, dass Änderungen an einer Komponente von allen Dokumententypen übernommen werden, welche diese Komponente verwenden.
//Falls Sie die Komponente "Titelzusätze" auch bei Dokumententypen nutzen, bei denen keine Ausgabe der Parallelveröffentlichungen erwünscht ist, gehen Sie folgendermaßen vor:
//Markieren Sie die Komponente und erzeugen über den Befehl "Komponente" > "Duplizieren" eine Kopie. Alternative: Erstellen Sie über das Menü "Komponente" > "Neu" eine neue Komponente.
//
//Schritt 2:
//Hinweise für die nötige Anpassung der betreffenden Titel im Citavi-Projekt:
//Verknüpfen Sie den Zeitschriftenartikel mit einem anderen Zeitschriftenartikel über die Registerkarte "Zusammenhang" > "Wechselseitige Verweise".
//WICHTIG: Der Verweis muss in beide Richtungen erfolgen, also sowohl "verweist auf..." als auch "wird verwiesen von ...".

using System;
using System.Text;
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
			handled = false;
			
			//make sure, the important variables have data
			if (citation == null) return null;
			
			Reference reference = citation.Reference;
			if (reference == null || reference.Project == null) return null;
			if (reference.ReferenceType != ReferenceType.JournalArticle) return null;
			
			if (!string.IsNullOrEmpty(reference.TitleSupplement)) return null;	//Diese Zeile ggf. durch // auskommentieren, falls auch bereits gefüllte Titelzusätze-Felder überschrieben werden sollen
			if (reference.Periodical == null) return null;
			
			
			//find the other reference via entity links
			if (reference.Project.EntityLinks == null || !reference.Project.EntityLinks.Any()) return null;

			List<Reference> referencesLinkedFromThis = 	(from link in reference.Project.EntityLinks.FindLinksForSource((ICitaviEntity)reference)
														where link.Target is Reference
														select (Reference)link.Target).ToList();
			
			if (referencesLinkedFromThis == null || !referencesLinkedFromThis.Any()) return null;
			
			List<Reference> referencesLinkingToThis = 	(from link in reference.Project.EntityLinks.FindLinksForTarget((ICitaviEntity)reference)
														where link.Source is Reference
														select (Reference)link.Source).ToList();
			
			if (referencesLinkingToThis == null || !referencesLinkingToThis.Any()) return null;
			
			//find first reference with a bidirectional link and treat that as the parrallel publication
			Reference otherReference = referencesLinkedFromThis.Intersect(referencesLinkingToThis).FirstOrDefault();
			if (otherReference == null) return null;
			if (otherReference.Periodical == null || string.IsNullOrEmpty(otherReference.Periodical.FullName)) return null;

			
			//prepare this component's output
			var output = new TextUnitCollection();
			StringBuilder outputTagged = new StringBuilder();
			
			
			#region title of the article		//wird für den Stil "Angewandte Chemie nicht benötigt
			
	/**		if (!string.IsNullOrEmpty(otherReference.Title))
			{
				if (output.Count > 0) 
				{
					outputTagged.Append(", ");
					output.Add(new LiteralTextUnit("; ", FontStyle.Neutral));
				}
				outputTagged.Append("<i>");
				outputTagged.Append(otherReference.Title);
				outputTagged.Append("</i>");
				output.Add(new LiteralTextUnit(otherReference.Title, FontStyle.Neutral));
			} **/
			
			#endregion
			
			#region periodical name 
			
			if (!string.IsNullOrEmpty(otherReference.Periodical.StandardAbbreviation))
			{
	/**			if (output.Count > 0) 					//wird benötigt, falls zuvor noch der Aufsatztitel ausgegeben wird
				{
					outputTagged.Append(", ");
					output.Add(new LiteralTextUnit(", ", FontStyle.Neutral));
				}**/
				outputTagged.Append("<i>");
				outputTagged.Append(otherReference.Periodical.StandardAbbreviation);
				outputTagged.Append("</i>");
				
				output.Add(new LiteralTextUnit(otherReference.Periodical.StandardAbbreviation, FontStyle.Italic));
				
			}
			else if (!string.IsNullOrEmpty(otherReference.Periodical.Name))
			{
	/**			if (output.Count > 0) 					//wird benötigt, falls zuvor noch der Aufsatztitel ausgegeben wird
				{
					outputTagged.Append(", ");
					output.Add(new LiteralTextUnit(", ", FontStyle.Neutral));
				}**/
				outputTagged.Append("<i>");
				outputTagged.Append(otherReference.Periodical.Name);
				outputTagged.Append("</i>");
				
				output.Add(new LiteralTextUnit(otherReference.Periodical.Name, FontStyle.Italic));
			}
			
			#endregion
			
			#region year
			
			if (!string.IsNullOrEmpty(otherReference.YearResolved))
			{
				if (output.Count > 0)
				{
					outputTagged.Append(", ");
					output.Add(new LiteralTextUnit(", ", FontStyle.Neutral));
	//				output.Add(new LiteralTextUnit(" (", FontStyle.Bold));
				}
				outputTagged.Append("<b>");
	//			outputTagged.Append(" (");			
				outputTagged.Append(otherReference.YearResolved);
	//			outputTagged.Append(")");
				outputTagged.Append("</b>");
				output.Add(new LiteralTextUnit(otherReference.YearResolved, FontStyle.Bold));
	//			output.Add(new LiteralTextUnit(")", FontStyle.Bold));
			}
			
			#endregion

			#region volumne number
			
			if (!string.IsNullOrEmpty(otherReference.Volume))
			{
				if (output.Count > 0) 
				{
					outputTagged.Append(", ");
					output.Add(new LiteralTextUnit(", ", FontStyle.Neutral));
				}
				outputTagged.Append("<i>");
				outputTagged.Append(otherReference.Volume);
				outputTagged.Append("</i>");
				output.Add(new LiteralTextUnit(otherReference.Volume, FontStyle.Italic));
			}
			
			#endregion
			
			#region page range
			
			if (!string.IsNullOrEmpty(otherReference.PageRange.ToString()))
			{
				if (output.Count > 0) 
				{
					outputTagged.Append(", ");
					output.Add(new LiteralTextUnit(", ", FontStyle.Neutral));
				}
				outputTagged.Append(otherReference.PageRange.ToString());
				output.Add(new LiteralTextUnit(otherReference.PageRange.ToString(), FontStyle.Neutral));
			}
			
			#endregion 
			
			#region DOI							//wird für den Stil "Angewandte Chemie nicht benötigt
			
	/**		if (!string.IsNullOrEmpty(otherReference.Doi))
			{
				if (output.Count > 0) 
				{
					outputTagged.Append(", https://doi.org/");
					output.Add(new LiteralTextUnit(", https://doi.org/", FontStyle.Neutral));
				}
			//	outputTagged.Append("<i>");
				outputTagged.Append(otherReference.Doi);
			//	outputTagged.Append("</i>");
				output.Add(new LiteralTextUnit(otherReference.Doi, FontStyle.Neutral));
			}	**/
			
			#endregion
			
			if (reference.Project.DesktopProjectConfiguration != null &&
				reference.Project.DesktopProjectConfiguration.Permissions != null &&
				reference.Project.DesktopProjectConfiguration.Permissions.DbPermission != null &&
				reference.Project.DesktopProjectConfiguration.Permissions.DbPermission == ProjectDbPermission.Write)
			{
				reference.TitleSupplementTagged = outputTagged.ToString();
			}
			
			handled = true;
			return output;
		}
	}
}
