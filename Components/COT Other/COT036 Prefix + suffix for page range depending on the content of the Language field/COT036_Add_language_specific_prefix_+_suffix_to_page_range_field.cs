//C6#COT036
//Description: Different prefix or suffix for page range depending on language of reference
//Version 1.1:	Slight improvements (instead of MarginMultiNumberingStyle, MarginTwoNumberingStyle was stated by mistake)
//Das Trennzeichen sowie die Formatierung von "Erste Zahl" und "Letzte Zahl" müssen in der Komponente selbst festgelegt werden.
//Auch die Optionen für Titel ohne Eintrag im Feld "Sprache" werden in den Komponenten-Einstellungen getroffen.

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
			
			if (citation == null) return null;
			if (componentPart == null) return null;
			if (componentPart.Elements == null) return null;
			if (!componentPart.Elements.Any()) return null;
			
			Reference reference = citation.Reference;
			if (reference == null) return null;
			
			Reference parentReference = reference.ParentReference;
			
			string language = string.Empty;
			if (componentPart.Scope == ComponentPartScope.Reference)
			{
				language = reference.Language.ToUpperInvariant();
			}
			else if (componentPart.Scope == ComponentPartScope.ParentReference)
			{
				if (parentReference == null) return null;
				language = parentReference.Language.ToUpperInvariant();				
			}
			
			if (string.IsNullOrEmpty(language)) return null;

			PageRangeFieldElement pageRangeFieldElement = componentPart.Elements.OfType<PageRangeFieldElement>().FirstOrDefault();
			if (pageRangeFieldElement == null) return null;

			PageRange pageRange = null;

			if (pageRangeFieldElement is QuotationPageRangeFieldElement)
			{
				PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
				if (placeholderCitation == null) return null;
				if (placeholderCitation.Entry == null) return null;

				pageRange = placeholderCitation.Entry.PageRange;

			}
			else if (pageRangeFieldElement is PageRangeFieldElement)
			{
				pageRange = citation.Reference.PageRange;
			}
			else
			{
				//what should that be ?
				return null;
			}

			if (pageRange == null) return null;

			#region German
			
			if (language.Contains("DE"))
			{
				
				#region Page
				
				pageRangeFieldElement.PageOneHasSpecialFormat = true;
				pageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageOnePrefix.Text = "S.\u00A0";
				pageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageOneSuffix.Text = "";
				pageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageTwoHasSpecialFormat = true;
				pageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageTwoPrefix.Text = "S.\u00A0";
				pageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageTwoSuffix.Text = "\u00A0f.";
				pageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageMultiPrefix.Text = "S.\u00A0";
				pageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageMultiSuffix.Text = "\u00A0ff.";
				pageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				pageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				pageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnOnePrefix.Text = "Sp.\u00A0";
				pageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnOneSuffix.Text = "";
				pageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				pageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnTwoPrefix.Text = "Sp.\u00A0";
				pageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0f.";
				pageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnMultiPrefix.Text = "Sp.\u00A0";
				pageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0ff.";
				pageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				pageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				pageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				pageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphOneSuffix.Text = "";
				pageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				pageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				pageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				pageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				pageRangeFieldElement.MarginOneHasSpecialFormat = true;
				pageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginOnePrefix.Text = "Rn.\u00A0";
				pageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginOneSuffix.Text = "";
				pageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				pageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginTwoPrefix.Text = "Rn.\u00A0";
				pageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0f.";
				pageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginMultiPrefix.Text = "Rn.\u00A0";
				pageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0ff.";
				pageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				pageRangeFieldElement.OtherOneHasSpecialFormat = false;
				pageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.OtherOnePrefix.Text = "";
				pageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherOneSuffix.Text = "";
				pageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				pageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherTwoPrefix.Text = "";
				pageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherTwoSuffix.Text = "";
				pageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				pageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherMultiPrefix.Text = "";
				pageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherMultiSuffix.Text = "";
				pageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				return null;
			}
			
			#endregion
			
			#region English
			
			if (language.Contains("EN"))
			{
				
				#region Page
				
				pageRangeFieldElement.PageOneHasSpecialFormat = true;
				pageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageOnePrefix.Text = "p.\u00A0";
				pageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageOneSuffix.Text = "";
				pageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageTwoHasSpecialFormat = true;
				pageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageTwoPrefix.Text = "pp.\u00A0";
				pageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageTwoSuffix.Text = "\u00A0f.";
				pageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageMultiPrefix.Text = "pp.\u00A0";
				pageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageMultiSuffix.Text = "\u00A0ff.";
				pageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				pageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				pageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnOnePrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnOneSuffix.Text = "";
				pageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				pageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnTwoPrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0f.";
				pageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnMultiPrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0ff.";
				pageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				pageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				pageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				pageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphOneSuffix.Text = "";
				pageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				pageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				pageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				pageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				pageRangeFieldElement.MarginOneHasSpecialFormat = true;
				pageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginOnePrefix.Text = "no.\u00A0";
				pageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginOneSuffix.Text = "";
				pageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				pageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginTwoPrefix.Text = "no.\u00A0";
				pageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0f.";
				pageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginMultiPrefix.Text = "no.\u00A0";
				pageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0ff.";
				pageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				pageRangeFieldElement.OtherOneHasSpecialFormat = false;
				pageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.OtherOnePrefix.Text = "";
				pageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherOneSuffix.Text = "";
				pageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				pageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherTwoPrefix.Text = "";
				pageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherTwoSuffix.Text = "";
				pageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				pageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherMultiPrefix.Text = "";
				pageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherMultiSuffix.Text = "";
				pageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				return null;
			}
			
			#endregion
			
			#region French
			
			if (language.Contains("FR"))
			{
				
				#region Page
				
				pageRangeFieldElement.PageOneHasSpecialFormat = true;
				pageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageOnePrefix.Text = "p.\u00A0";
				pageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageOneSuffix.Text = "";
				pageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageTwoHasSpecialFormat = true;
				pageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageTwoPrefix.Text = "p.\u00A0";
				pageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageTwoSuffix.Text = "\u00A0sq.";
				pageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageMultiPrefix.Text = "p.\u00A0";
				pageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageMultiSuffix.Text = "\u00A0sqq.";
				pageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				pageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				pageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnOnePrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnOneSuffix.Text = "";
				pageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				pageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnTwoPrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0sq.";
				pageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnMultiPrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0sqq.";
				pageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				pageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				pageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				pageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphOneSuffix.Text = "";
				pageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				pageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				pageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				pageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				pageRangeFieldElement.MarginOneHasSpecialFormat = true;
				pageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginOnePrefix.Text = "n°\u00A0";
				pageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginOneSuffix.Text = "";
				pageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				pageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginTwoPrefix.Text = "n°\u00A0";
				pageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0sq.";
				pageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginMultiPrefix.Text = "n°\u00A0";
				pageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0sqq.";
				pageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				pageRangeFieldElement.OtherOneHasSpecialFormat = false;
				pageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.OtherOnePrefix.Text = "";
				pageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherOneSuffix.Text = "";
				pageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				pageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherTwoPrefix.Text = "";
				pageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherTwoSuffix.Text = "";
				pageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				pageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherMultiPrefix.Text = "";
				pageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherMultiSuffix.Text = "";
				pageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				return null;
			}
			
			#endregion
			
			#region Italian
			
			if (language.Contains("IT"))
			{

				#region Page
				
				pageRangeFieldElement.PageOneHasSpecialFormat = true;
				pageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageOnePrefix.Text = "p.\u00A0";
				pageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageOneSuffix.Text = "";
				pageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageTwoHasSpecialFormat = true;
				pageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageTwoPrefix.Text = "p.\u00A0";
				pageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageTwoSuffix.Text = "\u00A0e\u00A0seg.";
				pageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageMultiPrefix.Text = "p.\u00A0";
				pageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageMultiSuffix.Text = "\u00A0e\u00A0segg.";
				pageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				pageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				pageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnOnePrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnOneSuffix.Text = "";
				pageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				pageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnTwoPrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0e\u00A0seg.";
				pageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnMultiPrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0e\u00A0segg.";
				pageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				pageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				pageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				pageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphOneSuffix.Text = "";
				pageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				pageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				pageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				pageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				pageRangeFieldElement.MarginOneHasSpecialFormat = true;
				pageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginOnePrefix.Text = "n.\u00A0marg.\u00A0";
				pageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginOneSuffix.Text = "";
				pageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				pageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginTwoPrefix.Text = "n.\u00A0marg.\u00A0";
				pageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0e\u00A0seg.";
				pageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginMultiPrefix.Text = "n.\u00A0marg.\u00A0";
				pageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0e\u00A0segg.";
				pageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				pageRangeFieldElement.OtherOneHasSpecialFormat = false;
				pageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.OtherOnePrefix.Text = "";
				pageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherOneSuffix.Text = "";
				pageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				pageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherTwoPrefix.Text = "";
				pageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherTwoSuffix.Text = "";
				pageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				pageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherMultiPrefix.Text = "";
				pageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherMultiSuffix.Text = "";
				pageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				return null;
			}
			
			#endregion

			#region Spanish
			
			if (language.Contains("ES"))
			{

				#region Page
				
				pageRangeFieldElement.PageOneHasSpecialFormat = true;
				pageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageOnePrefix.Text = "pág.\u00A0";
				pageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageOneSuffix.Text = "";
				pageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageTwoHasSpecialFormat = true;
				pageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageTwoPrefix.Text = "pág.\u00A0";
				pageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageTwoSuffix.Text = "\u00A0e\u00A0s.";
				pageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageMultiPrefix.Text = "pág.\u00A0";
				pageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageMultiSuffix.Text = "\u00A0e\u00A0ss.";
				pageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				pageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				pageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnOnePrefix.Text = "columna\u00A0";
				pageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnOneSuffix.Text = "";
				pageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				pageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnTwoPrefix.Text = "columna\u00A0";
				pageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0e\u00A0s.";
				pageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnMultiPrefix.Text = "columna\u00A0";
				pageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0e\u00A0ss.";
				pageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				pageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				pageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				pageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphOneSuffix.Text = "";
				pageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				pageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				pageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				pageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				pageRangeFieldElement.MarginOneHasSpecialFormat = true;
				pageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginOnePrefix.Text = "n.º\u00A0margin.\u00A0";
				pageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginOneSuffix.Text = "";
				pageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				pageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginTwoPrefix.Text = "n.º\u00A0margin.\u00A0";
				pageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0e\u00A0s.";
				pageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginMultiPrefix.Text = "n.º\u00A0margin.\u00A0";
				pageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0e\u00A0ss.";
				pageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				pageRangeFieldElement.OtherOneHasSpecialFormat = false;
				pageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.OtherOnePrefix.Text = "";
				pageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherOneSuffix.Text = "";
				pageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				pageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherTwoPrefix.Text = "";
				pageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherTwoSuffix.Text = "";
				pageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				pageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherMultiPrefix.Text = "";
				pageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherMultiSuffix.Text = "";
				pageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				return null;
			}
			
			#endregion
			
			#region Other: Language is NOT EMPTY - treat like English?
			
			if (!string.IsNullOrWhiteSpace(language));
			{
				
				#region Page
				
				pageRangeFieldElement.PageOneHasSpecialFormat = true;
				pageRangeFieldElement.PageOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageOnePrefix.Text = "p.\u00A0";
				pageRangeFieldElement.PageOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageOneSuffix.Text = "";
				pageRangeFieldElement.PageOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageTwoHasSpecialFormat = true;
				pageRangeFieldElement.PageTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageTwoPrefix.Text = "pp.\u00A0";
				pageRangeFieldElement.PageTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageTwoSuffix.Text = "\u00A0f.";
				pageRangeFieldElement.PageTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageMultiPrefix.Text = "pp.\u00A0";
				pageRangeFieldElement.PageMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.PageMultiSuffix.Text = "\u00A0ff.";
				pageRangeFieldElement.PageMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Column
				
				pageRangeFieldElement.ColumnOneHasSpecialFormat = true;
				pageRangeFieldElement.ColumnOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnOnePrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnOneSuffix.Text = "";
				pageRangeFieldElement.ColumnOneSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnTwoHasSpecialFormat = true;
				pageRangeFieldElement.ColumnTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnTwoPrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnTwoSuffix.Text = "\u00A0f.";
				pageRangeFieldElement.ColumnTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnMultiPrefix.Text = "col.\u00A0";
				pageRangeFieldElement.ColumnMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ColumnMultiSuffix.Text = "\u00A0ff.";
				pageRangeFieldElement.ColumnMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Paragraph
				
				pageRangeFieldElement.ParagraphOneHasSpecialFormat = true;
				pageRangeFieldElement.ParagraphOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ParagraphOnePrefix.Text = "§\u00A0";
				pageRangeFieldElement.ParagraphOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphOneSuffix.Text = "";
				pageRangeFieldElement.ParagraphOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				pageRangeFieldElement.ParagraphTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphTwoPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphTwoSuffix.Text = "";
				pageRangeFieldElement.ParagraphTwoSuffix.FontStyle = FontStyle.Neutral;
				
				pageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.ParagraphMultiPrefix.Text = "§§\u00A0";
				pageRangeFieldElement.ParagraphMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.ParagraphMultiSuffix.Text = "";
				pageRangeFieldElement.ParagraphMultiSuffix.FontStyle = FontStyle.Neutral;
				
				#endregion
				
				#region Margin
				
				pageRangeFieldElement.MarginOneHasSpecialFormat = true;
				pageRangeFieldElement.MarginOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginOnePrefix.Text = "no.\u00A0";
				pageRangeFieldElement.MarginOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginOneSuffix.Text = "";
				pageRangeFieldElement.MarginOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginTwoHasSpecialFormat = true;
				pageRangeFieldElement.MarginTwoNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginTwoPrefix.Text = "no.\u00A0";
				pageRangeFieldElement.MarginTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginTwoSuffix.Text = "\u00A0f.";
				pageRangeFieldElement.MarginTwoSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginMultiPrefix.Text = "no.\u00A0";
				pageRangeFieldElement.MarginMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.MarginMultiSuffix.Text = "\u00A0ff.";
				pageRangeFieldElement.MarginMultiSuffix.FontStyle = FontStyle.Neutral;				
				
				#endregion
				
				#region Other
				
				pageRangeFieldElement.OtherOneHasSpecialFormat = false;
				pageRangeFieldElement.OtherOneNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.OtherOnePrefix.Text = "";
				pageRangeFieldElement.OtherOnePrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherOneSuffix.Text = "";
				pageRangeFieldElement.OtherOneSuffix.FontStyle = FontStyle.Neutral;				
				
				pageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				pageRangeFieldElement.OtherTwoNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherTwoPrefix.Text = "";
				pageRangeFieldElement.OtherTwoPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherTwoSuffix.Text = "";
				pageRangeFieldElement.OtherTwoSuffix.FontStyle = FontStyle.Neutral;					
				
				pageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.FullRange;
				pageRangeFieldElement.OtherMultiPrefix.Text = "";
				pageRangeFieldElement.OtherMultiPrefix.FontStyle = FontStyle.Neutral;
				pageRangeFieldElement.OtherMultiSuffix.Text = "";
				pageRangeFieldElement.OtherMultiSuffix.FontStyle = FontStyle.Neutral;					
				
				#endregion
				
				return null;
			}
			
			#endregion
			
			return null;
		}
	}
}
