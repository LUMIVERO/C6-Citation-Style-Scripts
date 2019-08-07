//COT026
//In the output of text fields, replace round brackets with square brackets
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

            if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;
            if (citation == null || citation.Reference == null) return null;

            Reference reference = citation.Reference;
            if (componentPart.Scope == ComponentPartScope.ParentReference)
            {
                reference = citation.Reference.ParentReference;
            }
            if (reference == null) return null;



            foreach (TextFieldElement textFieldElement in componentPart.Elements.OfType<TextFieldElement>().ToList())
            {
                bool hasReplacements = false;

                string simpleValue = reference.GetValue(textFieldElement.PropertyId) as string;
                if (string.IsNullOrEmpty(simpleValue)) continue;


                var textUnits = textFieldElement.GetTextUnits(citation, template);
                var literalElements = textUnits.TextUnitsToLiteralElements(componentPart);

                if (simpleValue.Contains("("))
                {
                    hasReplacements = true;
                    foreach (LiteralElement element in literalElements)
                    {
                        element.Text = element.Text.Replace("(", "[");
                    }
                }

                if (simpleValue.Contains(")"))
                {
                    hasReplacements = true;
                    foreach (LiteralElement element in literalElements)
                    {
                        element.Text = element.Text.Replace(")", "]");
                    }
                }

                if (hasReplacements)
                {
                    componentPart.Elements.ReplaceItem(textFieldElement, literalElements);
                }
            }

            return null;
        }
	}
}
