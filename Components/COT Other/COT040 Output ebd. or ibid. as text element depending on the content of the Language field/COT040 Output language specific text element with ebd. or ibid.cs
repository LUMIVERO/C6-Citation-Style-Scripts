//COT040
//Description:	Output "ebd." or "ibid." as text element depending on the content of the "Language" field
//Version 1.0	

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
			handled = true;
			
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;
			
			#region Determine reference to look at & reference language
			
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
			
			#endregion Determine reference to look at
			
			string outputString = string.Empty;
			
			TextUnitCollection output = new TextUnitCollection();
			LiteralTextUnit text;

			#region German
			
			if (language.Contains("DE"))
			{
				outputString = "ebd.";
				handled = true;
			}
			
			#endregion 
			
			#region English
			
			else if (language.Contains("EN"))
			{
				outputString = "ibid.";
				handled = true;
			}
			
			#endregion 
			
			#region French
			
			else if (language.Contains("FR"))
			{
				outputString = "ibid.";
				handled = true;
			}
			
			#endregion 
			
			#region Other
			
			else
			{
				outputString = "ebd.";
				handled = true;
			}
			
			#endregion 

			text = new LiteralTextUnit(outputString, FontStyle.Neutral);
		//	text = new LiteralTextUnit(outputString, FontStyle.Bold | FontStyle.SmallCaps);

		//	output.Add(new LiteralTextUnit("[", FontStyle.Neutral));
			output.Add(text);
		//	output.Add(new LiteralTextUnit("]", FontStyle.Neutral));
			
			return output;
		}
	}
}
