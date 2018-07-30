//C6#COT004
//C5#431512
//Description: Suppress output of "Quotes page" if "Quotes page" is equal to the article's first page
//Version: 1.4  
//Version 1.4 Zeile 82 Reihenfolge der Transfersettings getauscht, damit Präfix von Zitat-Seiten ausgegeben wird
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

				return null;
			}
			

			var startPageArticle = citation.Reference.PageRange.StartPage;
			var startPageQuotation = placeholderCitation.Entry.PageRange.StartPage;

			//[2], [3], [4] [= nur Zitatseiten]
			if (startPageQuotation == startPageArticle)
			{
				//Alles unterdrücken ausser QuotationPageRangeFieldElement
				TransferSettings(quotationPageRangeFieldElement, pageRangeFieldElement);
				RemoveAllButElement(componentPart, quotationPageRangeFieldElement);

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
			pageRangeFieldElement = null;
			quotationPageRangeFieldElement = null;

			if (componentPart == null) return false;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return false;


			foreach (IElement element in componentPart.Elements)
			{
				if (element is QuotationPageRangeFieldElement)
				{
					if (quotationPageRangeFieldElement == null)
					{
						quotationPageRangeFieldElement = element as QuotationPageRangeFieldElement;
						continue;
					}
					else
					{
						//more than one QuotationPageRangeFieldElement
						quotationPageRangeFieldElement = null;
						pageRangeFieldElement = null;
						return false;
					}
				}

				if (element is PageRangeFieldElement)
				{
					if (pageRangeFieldElement == null)
					{
						pageRangeFieldElement = element as PageRangeFieldElement;
						continue;
					}
					else
					{
						//more than one PageRangeFieldElement
						pageRangeFieldElement = null;
						quotationPageRangeFieldElement = null;
						return false;
					}
				}

				if (element is FieldElement)
				{
					//we do not allow other FieldElements than 1 PageRangeFieldElement and 1 QuotationPageRangeFieldElement
					pageRangeFieldElement = null;
					quotationPageRangeFieldElement = null;
					return false;
				}
			}


			if (pageRangeFieldElement != null && quotationPageRangeFieldElement != null)
			{
				return true;
			}
			else
			{
				pageRangeFieldElement = null;
				quotationPageRangeFieldElement = null;
				return false;
			}

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

		void TransferSettings(PageRangeFieldElement sourceElement, PageRangeFieldElement targetElement)
		{
			targetElement.PageOnePrefix.Text = sourceElement.PageOnePrefix.Text;
			targetElement.PageOnePrefix.FontStyle = sourceElement.PageOnePrefix.FontStyle;

			targetElement.PageTwoPrefix.Text = sourceElement.PageTwoPrefix.Text;
			targetElement.PageTwoPrefix.FontStyle = sourceElement.PageTwoPrefix.FontStyle;

			targetElement.PageMultiPrefix.Text = sourceElement.PageMultiPrefix.Text;
			targetElement.PageMultiPrefix.FontStyle = sourceElement.PageMultiPrefix.FontStyle;

			targetElement.ColumnOnePrefix.Text = sourceElement.ColumnOnePrefix.Text;
			targetElement.ColumnOnePrefix.FontStyle = sourceElement.ColumnOnePrefix.FontStyle;

			targetElement.ColumnTwoPrefix.Text = sourceElement.ColumnTwoPrefix.Text;
			targetElement.ColumnTwoPrefix.FontStyle = sourceElement.ColumnTwoPrefix.FontStyle;

			targetElement.ColumnMultiPrefix.Text = sourceElement.ColumnMultiPrefix.Text;
			targetElement.ColumnMultiPrefix.FontStyle = sourceElement.ColumnMultiPrefix.FontStyle;

			targetElement.MarginOnePrefix.Text = sourceElement.MarginOnePrefix.Text;
			targetElement.MarginOnePrefix.FontStyle = sourceElement.MarginOnePrefix.FontStyle;

			targetElement.MarginTwoPrefix.Text = sourceElement.MarginTwoPrefix.Text;
			targetElement.MarginTwoPrefix.FontStyle = sourceElement.MarginTwoPrefix.FontStyle;

			targetElement.MarginMultiPrefix.Text = sourceElement.MarginMultiPrefix.Text;
			targetElement.MarginMultiPrefix.FontStyle = sourceElement.MarginMultiPrefix.FontStyle;

			targetElement.ParagraphOnePrefix.Text = sourceElement.ParagraphOnePrefix.Text;
			targetElement.ParagraphOnePrefix.FontStyle = sourceElement.ParagraphOnePrefix.FontStyle;

			targetElement.ParagraphTwoPrefix.Text = sourceElement.ParagraphTwoPrefix.Text;
			targetElement.ParagraphTwoPrefix.FontStyle = sourceElement.ParagraphTwoPrefix.FontStyle;

			targetElement.ParagraphMultiPrefix.Text = sourceElement.ParagraphMultiPrefix.Text;
			targetElement.ParagraphMultiPrefix.FontStyle = sourceElement.ParagraphMultiPrefix.FontStyle;

			targetElement.OtherOnePrefix.Text = sourceElement.OtherOnePrefix.Text;
			targetElement.OtherOnePrefix.FontStyle = sourceElement.OtherOnePrefix.FontStyle;

			targetElement.OtherTwoPrefix.Text = sourceElement.OtherTwoPrefix.Text;
			targetElement.OtherTwoPrefix.FontStyle = sourceElement.OtherTwoPrefix.FontStyle;

			targetElement.OtherMultiPrefix.Text = sourceElement.OtherMultiPrefix.Text;
			targetElement.OtherMultiPrefix.FontStyle = sourceElement.OtherMultiPrefix.FontStyle;
		}
	}
}