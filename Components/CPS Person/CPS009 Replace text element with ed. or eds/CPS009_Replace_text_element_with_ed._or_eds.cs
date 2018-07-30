//C6#CPS009
//C5#43120
//Description: Replace text element with ed. or eds
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
			handled = true;
			var output = new TextUnitCollection();


			if (citation == null) return output;
			if (citation.Reference == null) return output;

			if (!citation.Reference.HasCoreField(ReferenceTypeCoreFieldId.Editors)) return output; 	//empty string
			var editors = citation.Reference.Editors;
			if (editors == null || editors.Count == 0) return output;								//empty string

			if (editors.Count == 1)
			{
				output.Add(new LiteralTextUnit("ed."));
			}
			else
			{
				output.Add(new LiteralTextUnit("eds."));
			}

			return output;
		}
	}
}