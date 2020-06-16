//COT033
//Description: Switch between PMID and DOI  

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
			
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null || componentPart.Elements.Count == 0) return null;
			
			var hasDOI = !string.IsNullOrEmpty(citation.Reference.Doi);
			var hasOnlineAddress = !string.IsNullOrEmpty(citation.Reference.OnlineAddress);
			
			if (!hasDOI && !hasOnlineAddress) return null; //the component "clears" by itself, because the (static) label is only printed if one of the fields has content.
			
			
			IEnumerable<TextFieldElement> doiFieldElements = componentPart.Elements.OfType<TextFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.Doi);
			if (doiFieldElements == null || doiFieldElements.Count() != 1) return null;
			TextFieldElement doiFieldElement = doiFieldElements.FirstOrDefault();
			if (doiFieldElement == null) return null;
			
			
			
			IEnumerable<ElectronicAddressFieldElement> onlineAddressFieldElements = componentPart.Elements.OfType<ElectronicAddressFieldElement>().Where(element => element.PropertyId == ReferencePropertyId.OnlineAddress);
			if(onlineAddressFieldElements == null || onlineAddressFieldElements.Count() != 1) return null;
			ElectronicAddressFieldElement onlineAddressFieldElement = onlineAddressFieldElements.FirstOrDefault();
			if (onlineAddressFieldElement == null) return null;
			
			IEnumerable<LiteralElement> literalElements = componentPart.Elements.OfType<LiteralElement>();
			if (literalElements == null || literalElements.Count() != 1) return null;
			LiteralElement literalElement = literalElements.FirstOrDefault();
			if (literalElement == null) return null;
			
			
			//only OnlineAddress? -> No labeling
			if (hasOnlineAddress && !hasDOI)
			{
				literalElement.Text = literalElement.Text + "";
				return null;
			}
			
			//just DOI? -> labeling "DOI: "
			if (!hasOnlineAddress && hasDOI)
			{
				literalElement.Text = literalElement.Text + " DOI: ";
				return null;
			}
			
			//OnlineAddress AND DOI? -> DOI is preferable! Remove OnlineAddress element
			if (hasOnlineAddress && hasDOI)
			{
				literalElement.Text = literalElement.Text + " DOI: ";
				componentPart.Elements.Remove(onlineAddressFieldElement);
				return null;
			}
			
			
			//We were never supposed to come here, but who knows ...
			return null;
			
		}
	}
}
