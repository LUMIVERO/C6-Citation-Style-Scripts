//C6#COT004
//C5#431512
//Description: Suppresses article's pages when quotation starts on first page of article
//IMPORTANT: Ensure the component has one of the following structure:
//1.) [article's pages][quotation pages]   <- you can use prefix/suffix of [quotation pages] element to place it inside parentheses
//2.) [article's pages][literal][quotation pages][literal]   <- literals can contain parenthesis
//3.) [article's pages][literal][quotation pages]

//Version 1.7 Added Transfer of HasSpecialFormat to TransferSettings
//Version 1.6 Line 91/92: Added changed order of Transfersettings, to choose between the output of the prefix of article's pages and the prefix of quotation pages
//Version 1.5 Parenthesis around quotation pages can be introduced as separate literal elements or as part of prefix/suffix of the quotation page range field element.
//Version 1.4 Line 82: Changed order of Transfersettings, to ensure the prefix of quotation pages is output
//Version 1.3 Umstellung von OriginalString auf PrettyString

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
			//return handled = true if this macro generates the output (as a IEnumerable<ITextUnit>); the standard output will be suppressed
			//return handled = false if you want Citavi to produce the standard output; 
			//you can still manipulate the component part and its elements before letting Citavi generate the output with handled = false
			handled = false;

			/*
			AUSGABE-ZIELE
			------------------------------------------------------------------------------------------------------------------------
			[1] Aufsatz-Seiten: 123-130   	Zitatseiten: - 			> AUSGABE: 123-130 			[= nur Aufsatzseiten]
			[2] Aufsatz-Seiten: 123-130		Zitatseiten: 123		> AUSGABE: 123				[= nur Zitatseiten]
			[3] Aufsatz-Seiten: 123-130		Zitatseiten: 123-124	> AUSGABE: 123 f.			[= nur Zitatseiten]
			[4] Aufsatz-Seiten: 123-130		Zitatseiten: 123-125	> AUSGABE: 123 ff.			[= nur Zitatseiten]
			[5] Aufsatz-Seiten: 123-130		Zitatseiten: 125		> AUSGABE: 123 (125)		[= Erste Aufsatzseite (Zitatseiten)]
			[6] Aufsatz-Seiten: 123-130		Zitatseiten: 125-126	> AUSGABE: 123 (125 f.)		[= Erste Aufsatzseite (Zitatseiten)]
			[7] Aufsatz-Seiten: 123-130		Zitatseiten: 125-127	> AUSGABE: 123 (125 ff.)	[= Erste Aufsatzseite (Zitatseiten)]
			*/


			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;


			if (citation == null) return null;
			if (citation.Reference == null) return null;
			
			var placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return null;
			if (placeholderCitation.Entry == null) return null;

			//[1]
			if (placeholderCitation.Entry.PageRange == null) return null;
			if (string.IsNullOrEmpty(placeholderCitation.Entry.PageRange.PrettyString)) return null;	//ElementApplyConditions sorgen dafür, dass keine leeren Klammern angezeigt werden

			//enthält nur 1 PageRangeFieldElement und 1 QuotationPageRangeFieldElement und sonst lediglich LiteralElemente
			PageRangeFieldElement pageRangeFieldElement = null;
			QuotationPageRangeFieldElement quotationPageRangeFieldElement = null;
			
			if (!TryValidateComponentPartStructure(componentPart, out pageRangeFieldElement, out quotationPageRangeFieldElement)) return null;
			
			if 
			(
				!citation.Reference.HasCoreField(ReferenceTypeCoreFieldId.PageRange) ||
				citation.Reference.PageRange == null ||
				string.IsNullOrEmpty(citation.Reference.PageRange.PrettyString)
			)
			{
				//Alles unterdrücken ausser QuotationPageRangeFieldElement
				TransferSettings(pageRangeFieldElement, quotationPageRangeFieldElement);
				RemoveAllButElement(componentPart, quotationPageRangeFieldElement);
				RemoveParenthesesFromPrefixSuffix(quotationPageRangeFieldElement);

				return null;
			}
			

			var startPageArticle = citation.Reference.PageRange.StartPage;
			var startPageQuotation = placeholderCitation.Entry.PageRange.StartPage;

			//[2], [3], [4] [= nur Zitatseiten]
			if (startPageQuotation == startPageArticle)
			{
				//Alles unterdrücken ausser QuotationPageRangeFieldElement - wichtig: Eine der beiden TransferSettings-Zeilen muss immer durch vorangestellte //-Schrägstriche auskommentiert sein!
				//TransferSettings(quotationPageRangeFieldElement, pageRangeFieldElement);	//diese Reihenfolge verwenden, wenn die Präfixe der Zitatseiten-Komponente angezeigt werden sollen 
				TransferSettings(pageRangeFieldElement, quotationPageRangeFieldElement);	//diese Reihenfolge verwenden, wenn die Präfixe der Seitenbereichs-Komponente angezeigt werden sollen
				RemoveAllButElement(componentPart, quotationPageRangeFieldElement);
				RemoveParenthesesFromPrefixSuffix(quotationPageRangeFieldElement);

				return null;
			}

			//[5], [6], [7] [= Erste Aufsatzseite (Zitatseiten)]
			else
			{
				pageRangeFieldElement.PageOneHasSpecialFormat = false;
				pageRangeFieldElement.PageTwoHasSpecialFormat = false;
				pageRangeFieldElement.PageMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.PageMultiSuffix.Text = string.Empty;

				pageRangeFieldElement.ColumnOneHasSpecialFormat = false;
				pageRangeFieldElement.ColumnTwoHasSpecialFormat = false;
				pageRangeFieldElement.ColumnMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ColumnMultiSuffix.Text = string.Empty;

				pageRangeFieldElement.MarginOneHasSpecialFormat = false;
				pageRangeFieldElement.MarginTwoHasSpecialFormat = false;
				pageRangeFieldElement.MarginMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.MarginMultiSuffix.Text = string.Empty;

				pageRangeFieldElement.ParagraphOneHasSpecialFormat = false;
				pageRangeFieldElement.ParagraphTwoHasSpecialFormat = false;
				pageRangeFieldElement.ParagraphMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.ParagraphMultiSuffix.Text = string.Empty;

				pageRangeFieldElement.OtherOneHasSpecialFormat = false;
				pageRangeFieldElement.OtherTwoHasSpecialFormat = false;
				pageRangeFieldElement.OtherMultiNumberingStyle = NumberingStyle.StartPageOnly;
				pageRangeFieldElement.OtherMultiSuffix.Text = string.Empty;

				return null;
			}
		}

		bool TryValidateComponentPartStructure(ComponentPart componentPart, out PageRangeFieldElement pageRangeFieldElement, out QuotationPageRangeFieldElement quotationPageRangeFieldElement)
		{
			
			/*
				Allowed structures:
				/1/ [article pages] [quotation pages]
				/2/ [article pages] [literal] [quotation pages]
				/3/ [article pages] [literal] [quotation pages] [literal]
			*/
			
			pageRangeFieldElement = null;
			quotationPageRangeFieldElement = null;

			if (componentPart == null) return false;
			if (componentPart.Elements == null || 
				componentPart.Elements.Count == 0 || 
				componentPart.Elements.Count < 2 || 
				componentPart.Elements.Count > 4) return false;

			IElement element01 = componentPart.Elements.ElementAtOrDefault(0);
			IElement element02 = componentPart.Elements.ElementAtOrDefault(1);
			IElement element03 = componentPart.Elements.ElementAtOrDefault(2);
			IElement element04 = componentPart.Elements.ElementAtOrDefault(3);
			
			
			#region /1/ [article pages] [quotation pages]
			if (element04 == null &&
				element03 == null &&
				element02 != null &&
				element02 is QuotationPageRangeFieldElement &&
				element01 != null &&
				element01 is PageRangeFieldElement &&
				((PageRangeFieldElement)element01).PropertyId == ReferencePropertyId.PageRange)
			{
				pageRangeFieldElement = (PageRangeFieldElement)element01;
				quotationPageRangeFieldElement = (QuotationPageRangeFieldElement)element02;
				return true;
			}
			#endregion
			
			#region /2/ [article pages] [literal] [quotation pages]
			if (element04 == null &&
				element03 != null &&
				element03 is QuotationPageRangeFieldElement &&
				element02 != null &&
				element02 is LiteralElement &&
				element01 != null &&
				element01 is PageRangeFieldElement &&
				((PageRangeFieldElement)element01).PropertyId == ReferencePropertyId.PageRange)
			{
				pageRangeFieldElement = (PageRangeFieldElement)element01;
				quotationPageRangeFieldElement = (QuotationPageRangeFieldElement)element03;
				return true;
			}
			#endregion
			
			#region /3/ [article pages] [literal] [quotation pages] [literal]
			if (element04 != null &&
				element04 is LiteralElement &&
				element03 != null &&
				element03 is QuotationPageRangeFieldElement &&
				element02 != null &&
				element02 is LiteralElement &&
				element01 != null && 
				element01 is PageRangeFieldElement &&
				((PageRangeFieldElement)element01).PropertyId == ReferencePropertyId.PageRange)
			{
				pageRangeFieldElement = (PageRangeFieldElement)element01;
				quotationPageRangeFieldElement = (QuotationPageRangeFieldElement)element03;
				return true;
			}
			#endregion
			
			return false;
		}

		void RemoveAllButElement(ComponentPart componentPart, IElement elementToKeep)
		{
			if (componentPart == null || componentPart.Elements == null || componentPart.Elements.Count == 0) return;
			var elements = componentPart.Elements.ToList();

			foreach (IElement element in elements)
			{
				if (element == elementToKeep) continue;
				componentPart.Elements.Remove(element);
			}
		}
		
		void RemoveParenthesesFromPrefixSuffix(PageRangeFieldElement pageRangeFieldElement)
		{
			if (pageRangeFieldElement == null) return;
			
			LiteralElement[] prefixes = new LiteralElement[] {
				pageRangeFieldElement.PageOnePrefix,
				pageRangeFieldElement.PageTwoPrefix,
				pageRangeFieldElement.PageMultiPrefix,
				
				pageRangeFieldElement.ColumnOnePrefix,
				pageRangeFieldElement.ColumnTwoPrefix,
				pageRangeFieldElement.ColumnMultiPrefix,
				
				pageRangeFieldElement.ParagraphOnePrefix,
				pageRangeFieldElement.ParagraphTwoPrefix,
				pageRangeFieldElement.ParagraphMultiPrefix,
				
				pageRangeFieldElement.MarginOnePrefix,
				pageRangeFieldElement.MarginTwoPrefix,
				pageRangeFieldElement.MarginMultiPrefix,
				
				pageRangeFieldElement.OtherOnePrefix,
				pageRangeFieldElement.OtherTwoPrefix,
				pageRangeFieldElement.OtherMultiPrefix
			};
			
			LiteralElement[] suffixes = new LiteralElement[] { 
				pageRangeFieldElement.PageOneSuffix,
				pageRangeFieldElement.PageTwoSuffix,
				pageRangeFieldElement.PageMultiSuffix,
				
				pageRangeFieldElement.ColumnOneSuffix,
				pageRangeFieldElement.ColumnTwoSuffix,
				pageRangeFieldElement.ColumnMultiSuffix,
				
				pageRangeFieldElement.ParagraphOneSuffix,
				pageRangeFieldElement.ParagraphTwoSuffix,
				pageRangeFieldElement.ParagraphMultiSuffix,
				
				pageRangeFieldElement.MarginOneSuffix,
				pageRangeFieldElement.MarginTwoSuffix,
				pageRangeFieldElement.MarginMultiSuffix,
				
				pageRangeFieldElement.OtherOneSuffix,
				pageRangeFieldElement.OtherTwoSuffix,
				pageRangeFieldElement.OtherMultiSuffix
			};
			
			foreach(LiteralElement element in prefixes)
			{
				TrimStartParentheses(element);
			}
			foreach(LiteralElement element in suffixes)
			{
				TrimEndParentheses(element);
			}
			
		}
		
		void TrimStartParentheses(LiteralElement literalElement)
		{
			if (literalElement == null) return;
			if (string.IsNullOrEmpty(literalElement.Text)) return;
			
			char[] charsToTrim = {' ', '('};
			literalElement.Text = literalElement.Text.TrimStart(charsToTrim);
		}
		
		void TrimEndParentheses(LiteralElement literalElement)
		{
			if (literalElement == null) return;
			if (string.IsNullOrEmpty(literalElement.Text)) return;
			
			char[] charsToTrim = {' ', ')'};
			literalElement.Text = literalElement.Text.TrimEnd(charsToTrim);
		}
		

		void TransferSettings(PageRangeFieldElement sourceElement, PageRangeFieldElement targetElement)
		{
			#region Page
			
			targetElement.PageOneHasSpecialFormat = sourceElement.PageOneHasSpecialFormat;
			targetElement.PageOnePrefix.Text = sourceElement.PageOnePrefix.Text;
			targetElement.PageOnePrefix.FontStyle = sourceElement.PageOnePrefix.FontStyle;

			targetElement.PageTwoHasSpecialFormat = sourceElement.PageTwoHasSpecialFormat;
			targetElement.PageTwoPrefix.Text = sourceElement.PageTwoPrefix.Text;
			targetElement.PageTwoPrefix.FontStyle = sourceElement.PageTwoPrefix.FontStyle;

			targetElement.PageMultiPrefix.Text = sourceElement.PageMultiPrefix.Text;
			targetElement.PageMultiPrefix.FontStyle = sourceElement.PageMultiPrefix.FontStyle;

			#endregion
			
			#region Column
			
			targetElement.ColumnOneHasSpecialFormat = sourceElement.ColumnOneHasSpecialFormat;
			targetElement.ColumnOnePrefix.Text = sourceElement.ColumnOnePrefix.Text;
			targetElement.ColumnOnePrefix.FontStyle = sourceElement.ColumnOnePrefix.FontStyle;

			targetElement.ColumnTwoHasSpecialFormat = sourceElement.ColumnTwoHasSpecialFormat;
			targetElement.ColumnTwoPrefix.Text = sourceElement.ColumnTwoPrefix.Text;
			targetElement.ColumnTwoPrefix.FontStyle = sourceElement.ColumnTwoPrefix.FontStyle;

			targetElement.ColumnMultiPrefix.Text = sourceElement.ColumnMultiPrefix.Text;
			targetElement.ColumnMultiPrefix.FontStyle = sourceElement.ColumnMultiPrefix.FontStyle;

			#endregion
			
			#region Margin
			
			targetElement.MarginOneHasSpecialFormat = sourceElement.MarginOneHasSpecialFormat;
			targetElement.MarginOnePrefix.Text = sourceElement.MarginOnePrefix.Text;
			targetElement.MarginOnePrefix.FontStyle = sourceElement.MarginOnePrefix.FontStyle;

			targetElement.MarginTwoHasSpecialFormat = sourceElement.MarginTwoHasSpecialFormat;
			targetElement.MarginTwoPrefix.Text = sourceElement.MarginTwoPrefix.Text;
			targetElement.MarginTwoPrefix.FontStyle = sourceElement.MarginTwoPrefix.FontStyle;

			targetElement.MarginMultiPrefix.Text = sourceElement.MarginMultiPrefix.Text;
			targetElement.MarginMultiPrefix.FontStyle = sourceElement.MarginMultiPrefix.FontStyle;

			#endregion
			
			#region Paragraph
			
			targetElement.ParagraphOneHasSpecialFormat = sourceElement.ParagraphOneHasSpecialFormat;
			targetElement.ParagraphOnePrefix.Text = sourceElement.ParagraphOnePrefix.Text;
			targetElement.ParagraphOnePrefix.FontStyle = sourceElement.ParagraphOnePrefix.FontStyle;

			targetElement.ParagraphTwoHasSpecialFormat = sourceElement.ParagraphTwoHasSpecialFormat;
			targetElement.ParagraphTwoPrefix.Text = sourceElement.ParagraphTwoPrefix.Text;
			targetElement.ParagraphTwoPrefix.FontStyle = sourceElement.ParagraphTwoPrefix.FontStyle;

			targetElement.ParagraphMultiPrefix.Text = sourceElement.ParagraphMultiPrefix.Text;
			targetElement.ParagraphMultiPrefix.FontStyle = sourceElement.ParagraphMultiPrefix.FontStyle;

			#endregion
			
			#region Other
			
			targetElement.OtherOneHasSpecialFormat = sourceElement.OtherOneHasSpecialFormat;
			targetElement.OtherOnePrefix.Text = sourceElement.OtherOnePrefix.Text;
			targetElement.OtherOnePrefix.FontStyle = sourceElement.OtherOnePrefix.FontStyle;

			targetElement.OtherTwoHasSpecialFormat = sourceElement.OtherTwoHasSpecialFormat;
			targetElement.OtherTwoPrefix.Text = sourceElement.OtherTwoPrefix.Text;
			targetElement.OtherTwoPrefix.FontStyle = sourceElement.OtherTwoPrefix.FontStyle;

			targetElement.OtherMultiPrefix.Text = sourceElement.OtherMultiPrefix.Text;
			targetElement.OtherMultiPrefix.FontStyle = sourceElement.OtherMultiPrefix.FontStyle;
			
			#endregion
		}
	}
}
