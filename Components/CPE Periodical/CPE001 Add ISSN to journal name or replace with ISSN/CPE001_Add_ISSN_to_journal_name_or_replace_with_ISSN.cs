//C6#CPE001
//C5#43121
//Description: Adds the ISSN to the periodical name or replaces the periodical name for the ISSN
//Version: 2.1  
//Version 2.1	changed options to that component can also be suppressed altogether if there is no ISSN for a given Journal
//				therefore we now have the options (output modes) AppendISSNToJournalName, ShowISSNOnlyOrJournalName or ShowISSNOnlyOrSuppressJournalName
//Version 2.0	added option to append ISSN or replace periodical name with ISSN
//Version 1.1	added options to set parentheses around the ISSN as well as prefix and font styles for all elements, e.g.: , (ISSN)

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
			//-------- START OPTIONS ----------------
			var outputMode = OutputMode.ShowISSNOnlyOrSuppressJournalName; //append ISSN to journal name or replace the journal name with the ISSN or suppress it when there is no ISSN
			
			var prefix = "ISSN ";						//add a space e.g., or " ISSN: "
			var prefixFontStyle = FontStyle.Neutral;	//this is how you assign several styles at once: = FontStyle.Bold | FontStyle.Italic;
			
			var useBrackets = BracketType.None;			//replace None with either of the following: Parentheses, Brackes, CurlyBraces, Slashes, AngleBracktes
			var bracketsFontStyle = FontStyle.Neutral;
			
			var issnFontStyle = FontStyle.Neutral;			
			//-------- END OPTIONS ----------------

			
			handled = false;
			
			if (componentPart == null) return null;
			if (template == null) return null;
			if (citation == null || citation.Reference == null) return null;

			var periodical = citation.Reference.Periodical;
			if (periodical == null) return null;
			var issnString = periodical.Issn;
			bool hasISSN = !string.IsNullOrWhiteSpace(issnString);

			var periodicalFieldElement = componentPart.GetFieldElements().FirstOrDefault<FieldElement>(item => item.PropertyId == ReferencePropertyId.Periodical);
			if (periodicalFieldElement == null) return null;
			
			if (!hasISSN && outputMode == OutputMode.ShowISSNOnlyOrSuppressJournalName)
			{
				handled = true;
				return null;	//suppresses the component altogether
			}
			
			List<LiteralElement> issnElements = new List<LiteralElement>();

			#region Prefix
			
			if (!string.IsNullOrEmpty(prefix))
			{
				var prefixLiteralElement = new LiteralElement(componentPart, prefix);
				prefixLiteralElement.FontStyle = prefixFontStyle;
				issnElements.Add(prefixLiteralElement);
			}
			#endregion Prefix
			
			#region Opening Bracket
			
			string openingBracket = string.Empty;
			
			switch(useBrackets)
			{		
				case BracketType.AngleBrackets:
					openingBracket = "<";
					break;
					
				case BracketType.Brackets:
					openingBracket = "[";
					break;
					
				case BracketType.CurlyBraces:
					openingBracket = "{";
					break;
					
				case BracketType.Parentheses:
					openingBracket = "(";
					break;
					
				case BracketType.Slashes:
					openingBracket = "/";
					break;
					
				default:
					break;
			}
			
			if (hasISSN && !string.IsNullOrEmpty(openingBracket))
			{
				var openingBracketLiteralElement = new LiteralElement(componentPart, openingBracket);
				openingBracketLiteralElement.FontStyle = bracketsFontStyle;
				issnElements.Add(openingBracketLiteralElement);
			}
				
			#endregion Opening Bracket
			
			#region ISSN
			
			if (hasISSN)
			{
				var issnLiteralElement = new LiteralElement(componentPart, issnString);
				issnLiteralElement.FontStyle = issnFontStyle;
				issnElements.Add(issnLiteralElement);
			}
			
			#endregion ISSN
			
			#region Closing Bracket
			
			string closingBracket = string.Empty;
			
			switch(useBrackets)
			{
				case BracketType.AngleBrackets:
					closingBracket = ">";
					break;
					
				case BracketType.Brackets:
					closingBracket = "]";
					break;
					
				case BracketType.CurlyBraces:
					closingBracket = "}";
					break;
					
				case BracketType.Parentheses:
					closingBracket = ")";
					break;
					
				case BracketType.Slashes:
					closingBracket = "/";
					break;
					
				default:
					break;
			}
			
			if (hasISSN && !string.IsNullOrEmpty(closingBracket))
			{
				var closingBracketLiteralElement = new LiteralElement(componentPart, closingBracket);
				closingBracketLiteralElement.FontStyle = bracketsFontStyle;
				issnElements.Add(closingBracketLiteralElement);
			}

			#endregion Closing Bracket
			
			if (hasISSN)
			{
				int index = componentPart.Elements.IndexOf(periodicalFieldElement); 
				if (outputMode == OutputMode.AppendISSNToJournalName)
				{
					if (index == -1) index = componentPart.Elements.Count - 1;	//default: add at the end
					componentPart.Elements.InsertElements(index + 1, issnElements);
				}
				else
				{
					if (index != -1)
					{
						componentPart.Elements.ReplaceItem(index, issnElements);
						
						//always show LiteralElements
						var literalElements = componentPart.Elements.OfType<LiteralElement>();
						foreach (LiteralElement literalElement in literalElements)
						{
							literalElement.ApplyCondition = ElementApplyCondition.Always;
						}
					}
				}
			}
			
			handled = false;
			return null;
		}
		
		enum BracketType
		{
			None,
			Parentheses,
			Brackets,
			CurlyBraces,
			Slashes,
			AngleBrackets
		}
		
		enum OutputMode
		{
			AppendISSNToJournalName,
			ShowISSNOnlyOrJournalName,
			ShowISSNOnlyOrSuppressJournalName
		}
	}
}