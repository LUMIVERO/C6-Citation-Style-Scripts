//C6#COT015
//C5#431524
//Description: Suppress output of component part depending on quotation type
//Version: 1.0  

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

			bool doSuppressOnComments = true;				//Kommentare
			bool doSuppressOnIndirectQuotations = true; 	//Indirekte Zitate
			bool doSuppressOnSummaries = true;				//Zusammenfassungen
			bool doSuppressOnNonQuotations = false;			//Nachweis ohne Wissenselement (auch Bildzitate)
			bool doSuppressOnDirectQuotations = false;		//Wörtliche Zitate
			
			//do not edit
			handled = false;
			if (citation == null) return null;
			if (componentPart == null) return null;
			if (componentPart.Elements == null) return null;
			if (!componentPart.Elements.Any()) return null;
			
			PlaceholderCitation placeholderCitation = citation as PlaceholderCitation;
			if (placeholderCitation == null) return null;
			if (placeholderCitation.Entry == null) return null;
			
			if 
			(
				(doSuppressOnComments && placeholderCitation.Entry.QuotationType == QuotationType.Comment) ||
				(doSuppressOnIndirectQuotations && placeholderCitation.Entry.QuotationType == QuotationType.IndirectQuotation) ||
				(doSuppressOnSummaries && placeholderCitation.Entry.QuotationType == QuotationType.Summary) ||
				(doSuppressOnNonQuotations && placeholderCitation.Entry.QuotationType == QuotationType.None) ||
				(doSuppressOnDirectQuotations && placeholderCitation.Entry.QuotationType == QuotationType.DirectQuotation)
			)
			{
				handled = true;
				return null;
			}
			
			return null;
		}
	}
}