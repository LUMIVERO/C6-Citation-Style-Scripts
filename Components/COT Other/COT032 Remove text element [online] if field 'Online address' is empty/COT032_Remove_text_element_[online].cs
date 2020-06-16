//COT031
//Remove text element [online] if field 'Online address' is empty 
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
			
			var hasDOI = !string.IsNullOrEmpty(citation.Reference.Doi);
			var hasOnlineAddress = !string.IsNullOrEmpty(citation.Reference.OnlineAddress);
			
			if (!hasDOI && !hasOnlineAddress) return null; //Suppress output of this literal
			
			//Still here? OK, so there's an online address
			handled = false;
			return null;
		}
	}
}
