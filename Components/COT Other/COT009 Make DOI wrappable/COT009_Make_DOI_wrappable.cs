//C6#COT009
//C5#431517
//Description: Make DOI wrappable
//Version: 1.0  

using System.Linq;
using System.Text.RegularExpressions;
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
			
			bool addDxDoi = true;				// display dx.doi.org/ 
			bool addHttp = true;				// add http:// to DOI to generate link; only works if addDxDoi == true;
			
			var makeWrappable = true;		//replaced '/.-' by '/.-' plus zero-width breaking opportunity
			var zwb = ((char)8203).ToString();	//http://en.wikipedia.org/wiki/Zero-width_space \u200B' or ALT+08203
			
			if (componentPart == null) return null;
			if (citation == null || citation.Reference == null) return null;
			
			string doi = citation.Reference.Doi;
			if (string.IsNullOrEmpty(doi)) return null;
			
			var doiFieldElement = componentPart.GetFieldElements().FirstOrDefault(fieldElement => fieldElement.PropertyId == ReferencePropertyId.Doi);
			if (doiFieldElement == null) return null;
			
			string result = doi;
			
			if (addDxDoi) 
			{
				result = "dx.doi.org/" + result;
				if (addHttp)
				{
					result = "http://" + result;
				}
			}
						
			
			if (makeWrappable)
			{
				var pattern = @"([/_](?![/_:;~])|[/_][/_:;~])";		//first part: negative lookahead assertion for / not followed by any one in a class of other interpunctuation chars
				result = Regex.Replace(result, pattern, "$1" + zwb);
			}
			
			if (string.IsNullOrEmpty(result)) return null;
			
			var doiLiteralElement = new LiteralElement(componentPart, result);
			componentPart.Elements.ReplaceItem(doiFieldElement, doiLiteralElement);
			
			foreach (LiteralElement literalElement in componentPart.Elements.OfType<LiteralElement>())
			{
				literalElement.ApplyCondition = ElementApplyCondition.Always;
			}
			
			return null;

		}
	}
}