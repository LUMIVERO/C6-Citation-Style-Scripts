//TXY001

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;

namespace SwissAcademic.Citavi.Citations
{
	public class CustomTemplateCondition
		:
		ITemplateConditionMacro
	{
		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{

			if (citation == null) return false;
			if (citation.Reference == null) return false;
			if (citation.Reference.ReferenceType != ReferenceType.CourtDecision) return false;

			var field = citation.Reference.SpecificField3;

			//note: if you do not specify the whole word, but rather its first characters, do NOT use * as a wildcard, but
			//\\w*, which means "zero or more word characters"
			var wordList1 = new string[] {
				"Schreiben", 
				"Erlass", 
				"Verfügung"
			};

			var regEx1 = new Regex(@"\b(" + string.Join("|", wordList1) + @")\b", RegexOptions.IgnoreCase);
			if (regEx1.IsMatch(field)) return true;
			return false;
		}
	}
}
