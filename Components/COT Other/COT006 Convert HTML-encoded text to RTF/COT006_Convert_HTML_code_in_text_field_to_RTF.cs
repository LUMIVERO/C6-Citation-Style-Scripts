//C6#COT006
//C5#431615
//Description: Convert HTML-encoded text to RTF
//Version: 1.2 Added capability to detect and handle "TemporaryFontStyle" for individual words within fields  

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

			foreach (var textUnit in originalTextUnits)
			{

				var literalTextUnit = textUnit as LiteralTextUnit;
				if (literalTextUnit != null)
				{
					var text = literalTextUnit.Text;
					if (!string.IsNullOrEmpty(text))
					{
						var originalFontStyle = literalTextUnit.FontStyle;
						if (literalTextUnit.HasTemporaryFontStyle())
                        {
							originalFontStyle |= literalTextUnit.TemporaryFontStyle;

						}
						var newTextUnits = literalTextUnit.LiteralElement.TaggedTextToTextUnits(text, originalFontStyle);
						if (newTextUnits != null && newTextUnits.Count > 0)
						{
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
						if (fieldTextUnit.HasTemporaryFontStyle())
                        {
							originalFontStyle |= fieldTextUnit.TemporaryFontStyle;
                        }
						var newTextUnits = fieldTextUnit.FieldElement.TaggedTextToTextUnits(text, originalFontStyle);
						if (newTextUnits != null && newTextUnits.Count > 0)
						{
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
