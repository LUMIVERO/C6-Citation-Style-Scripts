//C6#COT006
//C5#431615
//Description: Convert HTML-encoded text to RTF
//Version: 1.1  

using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;
//using SwissAcademic.WordProcessing;

namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
		:
		IComponentPartFilter
	{
		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{

			handled = false;
			
			if (template == null) return null;
			
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null) return null;
			
			//let Citavi do the apply conditions on elements inside the component part
			var originalTextUnits = componentPart.GetTextUnitsUnfiltered(citation, template);
			if (originalTextUnits == null) return null;
			
			var outputTextUnits = new TextUnitCollection();
			
			foreach(var textUnit in originalTextUnits)
			{
				
				var literalTextUnit = textUnit as LiteralTextUnit;
				if (literalTextUnit != null)
				{
					var text = literalTextUnit.Text;
					if (!string.IsNullOrEmpty(text))
					{
						var originalFontStyle = literalTextUnit.FontStyle;
						var newTextUnits = literalTextUnit.LiteralElement.TaggedTextToTextUnits(text);
						if (newTextUnits != null && newTextUnits.Count > 0)
						{
							newTextUnits.ForEach(item => item.FontStyle ^= originalFontStyle);
							outputTextUnits.AddRange(newTextUnits);
						}
					}
					continue;
				}
				
				var fieldTextUnit = textUnit as FieldTextUnit;
				if (fieldTextUnit != null)
				{
					var text = fieldTextUnit.Text;
					if (!string.IsNullOrEmpty(text))
					{
						var originalFontStyle = fieldTextUnit.FontStyle;
						var newTextUnits = fieldTextUnit.FieldElement.TaggedTextToTextUnits(text);
						if (newTextUnits != null && newTextUnits.Count > 0)
						{
							newTextUnits.ForEach(item => item.FontStyle ^= originalFontStyle);
							outputTextUnits.AddRange(newTextUnits);
						}
					}
				}
			}
			
			handled = true;
			return outputTextUnits;
		}
	}
}